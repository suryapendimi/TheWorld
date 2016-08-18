using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AppController:Controller
    {
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _worldRepository;
        private ILogger<AppController> _logger;

        public AppController(IMailService mailService,IConfigurationRoot config,IWorldRepository worldRepository,ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _worldRepository = worldRepository;

            _logger = logger;


        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize] //system will prompt users if they are not loggedin.
        public IActionResult Trips()
        {

            try
            {
                var data = _worldRepository.GetAllTrips();
                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed in the Index of appController while getting Trips info : {ex.Message}");
                return Redirect("/error");
            }
        }
        public IActionResult Contact()
        {
            var data = new TheWorld.ViewModels.ContactViewModel();

            //throw new Exception("problem!");
            return View(data);
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if(model.Email.Contains("aol.com"))
            {
                ModelState.AddModelError("Email", "We do not support AOL email address");
                //in the above if u pass "" then the error would show up in Validation Summary
                //if we pass "Email" then would show up next to the control in the view.
            }
            if (ModelState.IsValid)
            {
                //throw new Exception("problem!");
                _mailService.SendMail(_config["MailSettings:ToAddress"], model.Email, "From Email The World!", model.Message);
                ModelState.Clear();
                ViewBag.UserMessage = "Message Sent";
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
    }
}
