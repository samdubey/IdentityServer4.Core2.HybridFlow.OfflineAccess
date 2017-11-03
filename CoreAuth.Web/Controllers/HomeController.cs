using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreAuth.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;

namespace CoreAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public async Task Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(this.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);
            await AuthenticationHttpContextExtensions.SignOutAsync(this.HttpContext, OpenIdConnectDefaults.AuthenticationScheme);
        }
        [Authorize]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public async Task<IActionResult> Shouts()
        {
            await RefreshTokensAync();

            var token = await AuthenticationHttpContextExtensions.GetTokenAsync(this.HttpContext, "access_token");

            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task RefreshTokensAync()
        {
            var authorizatinServerInformation =
                await DiscoveryClient.GetAsync("http://localhost:26421");

            var client = new TokenClient(authorizatinServerInformation.TokenEndpoint,
                "socialnetwork_code", "secret");

            var refreshToken = await AuthenticationHttpContextExtensions.GetTokenAsync(this.HttpContext, "refresh_token");

            var tokenResponse = await client.RequestRefreshTokenAsync(refreshToken);

            var identityToken = await AuthenticationHttpContextExtensions.GetTokenAsync(this.HttpContext, "id_token");

            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);

            var tokens = new[] {
                new AuthenticationToken
                {
                    Name=OpenIdConnectParameterNames.IdToken,
                    Value=tokenResponse.IdentityToken
                },
                new AuthenticationToken
                {
                    Name=OpenIdConnectParameterNames.AccessToken,
                    Value=tokenResponse.AccessToken
                },
                new AuthenticationToken
                {
                    Name=OpenIdConnectParameterNames.RefreshToken,
                    Value=tokenResponse.RefreshToken
                },
                new AuthenticationToken
                {
                    Name="expires_at",
                    Value=expiresAt.ToString("o",CultureInfo.InvariantCulture)
                }
            };

            //var authenticationInformation =HttpContext.Authentication.GetAuthenticateInfoAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var authenticationInformation = await AuthenticationHttpContextExtensions.AuthenticateAsync(this.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);
            authenticationInformation.Properties.StoreTokens(tokens);

            await AuthenticationHttpContextExtensions.SignInAsync(this.HttpContext,
                CookieAuthenticationDefaults.AuthenticationScheme,
                authenticationInformation.Principal,
                authenticationInformation.Properties);
        }
    }
}
