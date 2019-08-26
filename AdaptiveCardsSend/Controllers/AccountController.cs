using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MsalAndGraphSdk.Controllers
{

    public class AccountController : Controller
    {

        // テナント ID
        public static readonly string GraphTenantId = "";
        // アプリケーション ID
        public static readonly string GraphClientId = "";
        // アプリケーション シークレット
        public static readonly string GraphClientSecret = "";
        // スコープ
        public static readonly string[] GraphScope = new[]
        {
            "https://graph.microsoft.com/User.Read",
            "https://graph.microsoft.com/Mail.Send"
        };
        // リダイレクト URL
        public static readonly string RedirectUrl = "https://localhost:5001/Account/Callback";

        private IHttpClientFactory httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [ActionName("Authorize")]
        public async Task<IActionResult> AuthorizeAsync()
        {
            // 認証を行うための URL にリダイレクトします
            var clientApplicationOptions = new ConfidentialClientApplicationOptions()
            {
                TenantId = GraphTenantId,
                ClientId = GraphClientId,
                ClientSecret = GraphClientSecret,
                RedirectUri = RedirectUrl
            };
            var clientApplicationBuilder = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(clientApplicationOptions);
            var clientApplication = clientApplicationBuilder.Build();
            var requestUrl = await clientApplication.GetAuthorizationRequestUrl(GraphScope).ExecuteAsync();

            return this.Redirect(requestUrl.ToString());
        }

        [ActionName("Callback")]
        public async Task<IActionResult> CallbackAsync(string code, string error, [FromQuery(Name = "error_description")]string description, string resource, string state)
        {
            // アクセス トークンを取得します
            var clientApplicationOptions = new ConfidentialClientApplicationOptions()
            {
                TenantId = GraphTenantId,
                ClientId = GraphClientId,
                ClientSecret = GraphClientSecret,
                RedirectUri = RedirectUrl
            };
            var clientApplicationBuilder = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(clientApplicationOptions);
            var clientApplication = clientApplicationBuilder.Build();
            var authenticationResult = await clientApplication.AcquireTokenByAuthorizationCode(GraphScope, code).ExecuteAsync();

            // アクセス トークンをセッションに格納します
            var accessToken = authenticationResult.AccessToken;
            this.HttpContext.Session.Set("access_token", Encoding.UTF8.GetBytes(accessToken));

            return this.RedirectToAction("Index", "Home");
        }

    }

}
