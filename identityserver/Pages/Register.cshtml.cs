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
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IOptions<AppSettings> appSettings;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptions<AppSettings> appSettings)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.appSettings = appSettings;
        }
        public void OnGet()
        {

        }
        public void OnPost() {

        }
    }
}
