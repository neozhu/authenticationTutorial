using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using identityserver.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace identityserver.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IOptions<AppSettings> appSettings;
        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }
        public LoginModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptions<AppSettings> appSettings)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.appSettings = appSettings;
        }
        public async Task OnGet(string returnUrl="")
        {
            var externalProviders = await signInManager.GetExternalAuthenticationSchemesAsync();
            this.ReturnUrl = returnUrl;
            Console.WriteLine(ReturnUrl);
        }
        public void OnPost(LoginViewModel viewModel) {
            Console.WriteLine(viewModel);
             Redirect(viewModel.ReturnUrl);
        }
    }
}
