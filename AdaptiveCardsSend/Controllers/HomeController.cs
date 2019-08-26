using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
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

    public class HomeController : Controller
    {

        private IHttpClientFactory httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [ActionName("Index")]
        [HttpGet()]
        public async Task<IActionResult> IndexAsync()
        {
            if (this.HttpContext.Session.Keys.Contains("access_token"))
            {
                // 自身の情報を取得します
                var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(async msg =>
                {
                    await Task.Run(() =>
                    {
                        if (this.HttpContext.Session.TryGetValue("access_token", out var buffer))
                        {
                            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(buffer));
                        }
                    });
                }));
                var currentUser = await graphClient.Me.Request().GetAsync();
                this.ViewBag.IsSignIn = true;
                this.ViewBag.DisplayName = currentUser.DisplayName;
                this.ViewBag.Mail = currentUser.Mail;
            }
            else
            {
                this.ViewBag.IsSignIn = false;
            }
            return this.View();
        }

        [ActionName("Index")]
        [HttpPost()]
        public async Task<IActionResult> IndexAsync(string payload)
        {
            try
            {
                // 自身の情報を取得します
                var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(async msg =>
                {
                    await Task.Run(() =>
                    {
                        if (this.HttpContext.Session.TryGetValue("access_token", out var buffer))
                        {
                            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(buffer));
                        }
                    });
                }));
                var currentUser = await graphClient.Me.Request().GetAsync();

                // メールを送信します
                await graphClient.Me.SendMail(new Message()
                {
                    ToRecipients = new[] { new Recipient() { EmailAddress = new EmailAddress() { Address = currentUser.Mail } } },
                    Subject = "Adaptive Cards Send",
                    Body = new ItemBody()
                    {
                        Content = "<script type=\"application/adaptivecard+json\">" + payload + "</script>",
                        ContentType = BodyType.Html
                    }
                }).Request().PostAsync();

                this.TempData["Message"] = "Sending the card was succeeded.";
            }
            catch (Exception)
            {
                this.TempData["Message"] = "Sending the card was failed.";
            }

            return this.RedirectToAction("Index");
        }

    }

}
