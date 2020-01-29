using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace basic.App_Start
{
    public class CustomRequireClaim: IAuthorizationRequirement
    {
        public CustomRequireClaim(string claimType)
        {
            ClaimType = claimType;
        }

        public string ClaimType { get; private set; }
    }
    public class CustomRequireClaimHandler : AuthorizationHandler<CustomRequireClaim>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequireClaim requirement)
        {
            var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
            if (hasClaim) {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireCustomClaim(this AuthorizationPolicyBuilder builder)
        {

            builder.AddRequirements(new CustomRequireClaim(ClaimTypes.Email));
            return builder;
        }
    }


    public class OperationAuthizationRequirement : IAuthorizationRequirement
    {
        public OperationAuthizationRequirement(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
    public class CookieJarAuthizationHandler :
        AuthorizationHandler<OperationAuthizationRequirement,CookieJar>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthizationRequirement requirement,
            CookieJar cookieJar)
        {
            if (requirement.Name == CookieJarOperatins.Open)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
            }else if (requirement.Name == CookieJarOperatins.ComeNear)
            {
                if (context.User.HasClaim("Friend", "Good"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }

    public static class CookieJarOperatins
    {
        public static string Open = "Open";
        public static string TakeCookie = "TakeCookie";
        public static string ComeNear = "ComeNear";
        public static string Look = "Look";
    }

    public class CookieJar
    {
        public string Name { get; set; }
    }

    public static class CookieJarAuthOperations {
        public static OperationAuthizationRequirement Open = new OperationAuthizationRequirement(CookieJarOperatins.Open);
        public static OperationAuthizationRequirement ComeNear = new OperationAuthizationRequirement(CookieJarOperatins.ComeNear);
    }


    public class ClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var hasFriendClaim = principal.Claims.Any(x => x.Type == "Friend");
            if (!hasFriendClaim)
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("Friend", "Normal"));
            }
            return Task.FromResult(principal);
        }
    }
}
