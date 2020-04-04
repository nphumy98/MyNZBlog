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
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "My"),
                new Claim(ClaimTypes.Email, "nphumy89@gmail.com")
            };

            var claimIdentities = new ClaimsIdentity(claims, "CookiesAuth");

            var userPrincipal = new ClaimsPrincipal(claimIdentities);

            await HttpContext.SignInAsync("CookiesAuth", userPrincipal, new AuthenticationProperties { IsPersistent = userModel.RememberMe });
            return LocalRedirect(userModel.ReturnUrl);
        }
    }
}
