﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace basic.App_Start
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        //private readonly IUserRepository _userRepository;

        public CustomCookieAuthenticationEvents(/*IUserRepository userRepository*/)
        {
            // Get the database from registered DI services.
            //_userRepository = userRepository;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            // Look for the LastChanged claim.
            var lastChanged = (from c in userPrincipal.Claims
                               where c.Type == "LastChanged"
                               select c.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(lastChanged)
               )
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
