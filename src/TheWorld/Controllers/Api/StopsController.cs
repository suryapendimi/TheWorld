using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TheWorld.Controllers.Api
{
    [Route("/api/trips/{tripName}/stops")]
    [Authorize]
    public class StopsController : Controller
    {
        private IWorldRepository _worldRepository;
        private ILogger<StopsController> _logger;
        private GeoCoordsService _geoService;

        public StopsController(IWorldRepository worldRepository,ILogger<StopsController> logger, GeoCoordsService geoService)
        {
            _worldRepository = worldRepository;
            _logger = logger;
            _geoService = geoService;
        }
        // GET: api/values
        [HttpGet("")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var results = _worldRepository.GetTripByName(tripName,User.Identity.Name);
                if(results ==null)
                {
                    return Json(null);
                }
                return Json(Mapper.Map<IEnumerable<StopViewModel>>(results.Stops.OrderBy(s => s.Order).ToList()));
                //return Ok(); //sending a ViewModel in to the View using a Mapper ext.
                //we need to configure in startup to reversemap (bidirectional Map)
            }
            catch (Exception ex)
            {
                //TODO Logging
                //we have added a LoggingFactory in startup class to write to Debug window/.
                _logger.LogError($"Failed to get All stops:{0}", ex);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Error occurred finding trip name");
            }

          
        }

        [HttpPost]
        public async Task<IActionResult> Post(string tripName,[FromBody]StopViewModel vm)
        {
            try {

                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm);
                    //Lookup the GeoCodes
                    var result = await _geoService.GetCoordsAsync(newStop.Name);
                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else
                    {
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;

                        _worldRepository.AddStop(tripName,User.Identity.Name, newStop);
                        //Save to the Database
                        //if (await _worldRepository.SaveChangesAsync())
                        //{
                        //    return Created($"/api/trips/{tripName}/stops/{newStop.Name}",
                        //    Mapper.Map<StopViewModel>(newStop));
                        //}
                        if(_worldRepository.SaveAll())
                        {
                            Response.StatusCode = (int)HttpStatusCode.Created;
                            return Json(Mapper.Map<StopViewModel>(newStop));

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save new Stop:{0}", ex);
            }
            return BadRequest("Failed to get stops");
        }
        //// POST api/values
        //[HttpPost=""]
        //public async Task<IActionResult> Post(string tripName,[FromBody]StopViewModel vm)
        //{
        //    try
        //    {
        //        //If the VM is valid
        //        if (ModelState.IsValid)
        //        {
        //            var newStop = Mapper.Map<Stop>(vm);
        //            //Lookup the GeoCodes

        //            //Save to the Database
        //            _worldRepository.AddStop(tripName, newStop);
        //            if (await _worldRepository.SaveChangesAsync())
        //            {
        //                return Created($"api/trips/{tripName}/stops/{newStop.Name}",
        //                    Mapper.Map<StopViewModel>(newStop));
        //            }
        //        }
        //        else
        //        {
        //            BadRequest("Failed to while getting Stops info");

        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        
    }
}
