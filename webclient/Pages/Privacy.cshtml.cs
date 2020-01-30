using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace webclient.Pages
{
    [Authorize]
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IHttpClientFactory httpClientFactory;

        public PrivacyModel(ILogger<PrivacyModel> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var claims = User.Claims.ToList();
            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var result = await GetSecret(accessToken);

            await RefreshAccessToken();
        }
        public async Task<string> GetSecret(string accessToken)
        {
            var apiClient = httpClientFactory.CreateClient();

            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.GetAsync("https://localhost:44337/secret");

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
        private async Task RefreshAccessToken()
        {
            var serverClient = httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44305/");

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var refreshTokenClient = httpClientFactory.CreateClient();

            var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    RefreshToken = refreshToken,
                    ClientId = "client_page",
                    ClientSecret = "client_secret"
                });

            var authInfo = await HttpContext.AuthenticateAsync("Cookie");

            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("id_token", tokenResponse.IdentityToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            await HttpContext.SignInAsync("Cookie", authInfo.Principal, authInfo.Properties);
        }

    }
}
