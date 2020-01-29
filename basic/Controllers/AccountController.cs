using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using basic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace basic.Controllers 
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // GET: /<controller>/
        public IActionResult Login(string ReturnUrl="")
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel) {
            //var claims = new List<Claim>()
            //{
            //    new Claim(ClaimTypes.Name,viewModel.Email),
            //    new Claim(ClaimTypes.Email,viewModel.Email),
            //    new Claim(ClaimTypes.Role,"admin"),
            //    new Claim("LastChanged",DateTime.UtcNow.ToString())

            //};
            //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //var principal = new ClaimsPrincipal(identity);
            //var authProperties = new AuthenticationProperties
            //{
            //    IsPersistent = viewModel.IsPersistent,
            //    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
            //};

            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal,authProperties);
            var user = await this._userManager.FindByEmailAsync(viewModel.Email);
            if (user != null) {
                var result = await this._signInManager.PasswordSignInAsync(user.UserName, viewModel.Password,viewModel.IsPersistent,false);
                if (result.Succeeded) {
                    return Redirect("/Home/Index");
                }
            }
            return View();
        }
        public async Task<IActionResult> Logout() {

            //this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           await  this._signInManager.SignOutAsync();
            return Redirect("/Home/Index");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            var user = new IdentityUser();
            user.Email = viewModel.Email;
            user.UserName = viewModel.Email;
            
            var result= await  this._userManager.CreateAsync(user,viewModel.Password);

            if (result.Succeeded) {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name,viewModel.Email),
                    new Claim(ClaimTypes.Email,viewModel.Email),
                    new Claim(ClaimTypes.Role,"admin"),
                    new Claim(ClaimTypes.Role,"users"),
                    new Claim("LastChanged",DateTime.UtcNow.ToString())
                };
                await this._userManager.AddClaimsAsync(user, claims);
                return Redirect("/Account/Login");
            }
            return View();
        }
        public IActionResult AccessDenied(string ReturnUrl="") {

            return View();
        }
    }
}
