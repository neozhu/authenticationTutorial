using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using basic.App_Start;
using basic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace basic.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppSettings _appSettings;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
        }
        // GET: /<controller>/
        public IActionResult Login(string ReturnUrl = "")
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
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
            if (user != null)
            {
                var result = await this._signInManager.PasswordSignInAsync(user.UserName, viewModel.Password, viewModel.IsPersistent, false);
                if (result.Succeeded)
                {
                    return Redirect("/Home/Index");
                }
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {

            //this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await this._signInManager.SignOutAsync();
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

            var result = await this._userManager.CreateAsync(user, viewModel.Password);

            if (result.Succeeded)
            {
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
        public IActionResult AccessDenied(string ReturnUrl = "")
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> token(string username, string password)
        {

            var user = await this._userManager.FindByEmailAsync(username);
            if (user != null)
            {
                var result = await this._signInManager.CheckPasswordSignInAsync(user, password, false);
                if (result.Succeeded)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        //    Subject = new ClaimsIdentity(new Claim[]
                        //    {
                        //new Claim(ClaimTypes.Name, user.Id.ToString())
                        //    }),
                        Subject = new ClaimsIdentity(await _userManager.GetClaimsAsync(user)),
                        Expires = DateTime.UtcNow.AddDays(30),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    //user.Token = tokenHandler.WriteToken(token);
                    var tokenstr = tokenHandler.WriteToken(token);
                    return Ok(new { access_token = tokenstr });
                }
            }
            return Ok();
        }


        [HttpGet]
        public IActionResult OAuth_Login(
            OAuthRequest request
            /*string response_type, // authorization flow type 
            string client_id, // client id
            string redirect_uri,
            string scope, // what info I want = email,grandma,tel
            string state*/
            ) {
            //var query = new QueryBuilder();
            //query.Add("redirectUri", request.Redirect_Uri);
            //query.Add("state", request.State);
            var loginViewModel = new OAuthLoginViewModel()
            {
                RedirectUri = request.Redirect_Uri,
                State = request.State
            };
            return View(model: loginViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> OAuth_Login(OAuthLoginViewModel viewModel)
        {

            var user = await this._userManager.FindByEmailAsync(viewModel.Email);
            if (user != null)
            {
                var result = await this._signInManager.CheckPasswordSignInAsync(user, viewModel.Password, false);
                if (result.Succeeded)
                {
                    var query = new QueryBuilder();
                    query.Add("code", user.Id);
                    query.Add("state", viewModel.State);
                    return Redirect($"{viewModel.RedirectUri}{query.ToString()}");
                }
            }
            return View();
        }
        public async Task<IActionResult> OAuth_Token(
            TokenRequest request
            /*string grant_type, // flow of access_token request
            string code, // confirmation of the authentication process
            string redirect_uri,
            string client_id,
            string refresh_token*/)
        {

            var user = await this._userManager.FindByIdAsync(request.Code);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //    Subject = new ClaimsIdentity(new Claim[]
                //    {
                //new Claim(ClaimTypes.Name, user.Id.ToString())
                //    }),
                Subject = new ClaimsIdentity(await _userManager.GetClaimsAsync(user)),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var access_token = tokenHandler.WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "oauthTutorial",
                refresh_token = "RefreshTokenSampleValueSomething"
            };

            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

            await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

            return Redirect(request.Redirect_Uri);
            
        }
    }
}
