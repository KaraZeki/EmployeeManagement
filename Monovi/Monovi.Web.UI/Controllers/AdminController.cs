using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monovi.Business.IServices;
using Monovi.Model.Concrete;
using Monovi.Model.Model;
using Monovi.Web.UI.Models;
using System.Collections.Generic;

namespace Monovi.Web.UI.Controllers
{
    //[Authorize]
    public class AdminController : Controller
    {
        private readonly IUserService _userfService;
        private IMapper _mapper;
        public AdminController(IUserService userfService, IMapper mapper)
        {
            _userfService = userfService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserList()
        {
            var userList = _userfService.GetUserList();

            var model = new UserListModel()
            {
                Users = _mapper.Map<List<UserViewModel>>(userList)
            };
            return View(model);
        }    
        
        public IActionResult DeleteUser(int id)
        {
            _userfService.DeleteUser(id);
            return RedirectToAction("UserList", "Admin");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
           var user= _userfService.GetUser(id);
           var model = _mapper.Map< UserViewModel>(user);
            return View(model);
        }

        [HttpPost]
        public IActionResult EditUser(UserViewModel user)
        {
            var userResult=_userfService.UpdateUser(_mapper.Map<User>(user));
            var model = _mapper.Map<UserViewModel>(userResult);
            return View(model);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }


        [HttpPost]
        public IActionResult AddUser(UserViewModel user)
        {
            _userfService.AddUser(_mapper.Map<User>(user));
            return RedirectToAction("UserList", "Admin");
        }

    }
}
