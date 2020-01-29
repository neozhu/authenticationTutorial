using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using basic.App_Start;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace basic.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IOptions<AppSettings> _config;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> config )
        {
            _authorizationService = authorizationService;
            _config = config;
            _userManager = userManager;
        }
        [AllowAnonymous]
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy() {
            return View();
        }
        [Authorize(Policy ="role")]
        public IActionResult Policy()
        {
            return View();
        }
        [Authorize(Policy = "admin")]
        public IActionResult Admin()
        {
            return View();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Users()
        {
            var users = await this._userManager.Users.ToListAsync();
            return Ok(users);
        }
        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View();
        }
        public async Task<IActionResult> DoStuff() {
            //var defaultAuthbuilder = new AuthorizationPolicyBuilder();
            //var defaultAuthPolicy = defaultAuthbuilder
            //.RequireAuthenticatedUser()
            //.RequireClaim(claimType:ClaimTypes.Email)
            //.Build();
            //var result = await this._authorizationService.AuthorizeAsync(User, defaultAuthPolicy);
            var result = await this._authorizationService.AuthorizeAsync(User, "DoStuff");
            if (result.Succeeded)
            {
                //to do something;
            }

            return View();
        }
        public async Task<IActionResult> Open()
        {
            var cookieJar = new CookieJar();
            cookieJar.Name = "Open";
            //var requirement = new OperationAuthizationRequirement(CookieJarOperatins.Open);
            //var result = await this._authorizationService.AuthorizeAsync(User, cookieJar, requirement);

            var result = await this._authorizationService.AuthorizeAsync(User, cookieJar, CookieJarAuthOperations.Open);
            if (result.Succeeded)
            {
                //to do something
                return View();
            }
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> ComeNear()
        {
            var cookieJar = new CookieJar();
            cookieJar.Name = "ComeNear";
            //var requirement = new OperationAuthizationRequirement(CookieJarOperatins.Open);
            //var result = await this._authorizationService.AuthorizeAsync(User, cookieJar, requirement);

            var result = await this._authorizationService.AuthorizeAsync(User, cookieJar, CookieJarAuthOperations.ComeNear);
            if (result.Succeeded)
            {
                //to do something
                return View();
            }
            return RedirectToAction("Index");

        }
    }
}
