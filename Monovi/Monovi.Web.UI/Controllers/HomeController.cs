using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Monovi.Business.IServices;
using Monovi.Model.Model;
using Monovi.Web.UI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Monovi.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;

        }

        public IActionResult Index()
        {
            return View();
        }

     

        public IActionResult MyAccount()
        {
            var id = int.Parse(User.FindFirst("Id").Value);
            var user = _userService.GetUser(id);
            var model = _mapper.Map<UserViewModel>(user);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
