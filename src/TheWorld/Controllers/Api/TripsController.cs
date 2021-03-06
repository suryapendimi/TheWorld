﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("Api/Trips")]
    [Authorize]
    public class TripsController: Controller
    {
        private ILogger<TripsController> _logger;
        private IWorldRepository _worldRepository;

        public TripsController(IWorldRepository worldRepository,ILogger<TripsController> logger)
        {
            _worldRepository = worldRepository;
            _logger = logger;
        }
        [HttpGet("")]
        public JsonResult Get()
        {            
            try
            {
                var trips = _worldRepository.GetUserTripsWithStops(User.Identity.Name);//GetAllTripsWithStops();
                    //GetUserTripsWithStops(User.Identity.Name);

            var results=Mapper.Map<IEnumerable<TripViewModel>>(trips); //sending a ViewModel in to the View using a Mapper ext.
                return Json(results);
            }
            catch(Exception ex)
            {
                //TODO Logging
                //we have added a LoggingFactory in startup class to write to Debug window/.
                _logger.LogError($"Failed to get All Trips:{ex}");

                return Json(null);

            }
        }

        [HttpPost("")]
        public JsonResult Post([FromBody] TripViewModel trip)
        {
            try
            {
                //if (true) return BadRequest("Bad things happend");
                //else
                if (ModelState.IsValid)
                {
                    ////Save to the Database 
                    ////1. convert new model to Trip
                    ////We can do this way but AutoMapper can help in a better way
                    //var newTrip = new Trip()
                    //{
                    //    Name = trip.Name,
                    //    DateCreated = trip.Created
                    //};
                    //Using AutoMapper class
                    var newTrip = Mapper.Map<Trip>(trip);
                    newTrip.UserName = User.Identity.Name;

                    _logger.LogInformation("Attempting to save new trip");
                    _worldRepository.AddTrip(newTrip);
                    //return Created($"api/trips/{trip.Name}", Mapper.Map<TripViewModel>(newTrip)); //to do 2-way mapping used a ReverseMap() in startup.cs class
                    if (_worldRepository.SaveAll())
                    {
                        Response.StatusCode = (int)HttpStatusCode.Created;
                        return Json(Mapper.Map<TripViewModel>(newTrip));
                        // return Created($"api/trips/{trip.Name}", Mapper.Map<TripViewModel>(newTrip));
                    }
                    else
                    {
                        _logger.LogError("Failed to save trips from line80 ");
                        //return BadRequest("Failed to Save changes to the database");
                        return Json(null);
                    }

                }
                else
                {
                    _logger.LogError("Invalid ModelState save All Trips");
                    return Json(null);
                    //return BadRequest(ModelState); //passing modelstate is not a good idea on a public API.
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save All Trips:{ex}");
                return Json(null);
            }

        }
    }
}
