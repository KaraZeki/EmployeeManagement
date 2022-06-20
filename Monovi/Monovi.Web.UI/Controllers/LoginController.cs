using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monovi.Business.IServices;
using Monovi.Model.Model;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Monovi.Web.UI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        public LoginController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index() => View();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserViewModel model)
        {

            var userCheck = _userService.UserAccountControl(model);
            if (userCheck.IsOk)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email,model.Email),
                    new Claim("Id",userCheck.Id.ToString())
                };
                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                await HttpContext.SignInAsync(principal);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Message"] = userCheck.Message;
                return RedirectToAction("Index", "Login");
            }

        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}
