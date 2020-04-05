using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyNZBlog.Data;
using MyNZBlog.Models;
using MyNZBlog.Models.ViewModel;

namespace MyNZBlog.Controllers
{
    //todo set authentication hander for cookies name string
    public class AdminController : Controller
    {
        private readonly MyNZBlogContext _context;

        public AdminController(MyNZBlogContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            var userLoginModel = new UserLoginModel()
            {
                ReturnUrl = returnUrl
            };
            return View(userLoginModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginModel userModel)
        {
            User user = _context.User.SingleOrDefault(u =>
                    u.IsAdmin == true && u.Email == userModel.User.Email && u.Password == userModel.User.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "My"),
                new Claim(ClaimTypes.Email, "nphumy89@gmail.com")
            };
            // the string CookiesAuth is scheme that need to match with AddCookies in 
            var claimIdentities = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var userPrincipal = new ClaimsPrincipal(claimIdentities);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties { IsPersistent = userModel.RememberMe });
            return LocalRedirect(userModel.ReturnUrl);
        }
    }
}
