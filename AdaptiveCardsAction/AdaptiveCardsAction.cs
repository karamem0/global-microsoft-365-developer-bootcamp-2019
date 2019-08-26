using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AdaptiveCardsAction
{

    public static class AdaptiveCardsAction
    {

        // テナント ID
        private static readonly string GraphTenantId = "";
        // アプリケーション ID
        private static readonly string GraphClientId = "";
        // アプリケーション シークレット
        private static readonly string GraphClientSecret = "";
        // スコープ
        private static readonly string[] GraphScope = new[]
        {
            "https://graph.microsoft.com/.default"
        };

        // OneDrive for Bussiness のユーザー プリンシパル名 (username@tenant.onmicrosoft.com)
        private static readonly string O4BUserPrincipalName = "";
        // OneDrive for Bussiness の Excel ファイル名
        private static readonly string O4BExcelFileName = "アンケート集計結果.xlsx";
        // OneDrive for Bussiness の Excel ファイルのテーブル名
        private static readonly string O4BExcelTableName = "テーブル1";

        [FunctionName("AdaptiveCardsAction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "POST")]HttpRequest req, ILogger log)
        {
            try
            {
                var serializer = new JsonSerializer();
                var message = serializer.Deserialize<JToken>(new JsonTextReader(new StreamReader(req.Body)));
                var values = JToken.FromObject(new[]
                {
                    new[]
                    {
                        message.Value<string>("name"),
                        message.Value<string>("rating"),
                        message.Value<string>("comment")
                    }
                });

                var clientApplicationOptions = new ConfidentialClientApplicationOptions()
                {
                    TenantId = GraphTenantId,
                    ClientId = GraphClientId,
                    ClientSecret = GraphClientSecret,
                };
                var clientApplicationBuilder = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(clientApplicationOptions);
                var clientApplication = clientApplicationBuilder.Build();
                var authenticationResult = await clientApplication.AcquireTokenForClient(GraphScope).ExecuteAsync();

                var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(async msg =>
                {
                    await Task.Run(() =>
                    {
                        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                    });
                }));

                var o4bUser = graphClient.Users[O4BUserPrincipalName];
                var o4bExcelFile = o4bUser.Drive.Root.ItemWithPath(O4BExcelFileName);
                var o4bExcelTable = o4bExcelFile.Workbook.Tables[O4BExcelTableName];
                await o4bExcelTable.Rows.Add(values: values).Request().PostAsync();

                req.HttpContext.Response.Headers.Add("CARD-ACTION-STATUS", "Responding to the survey was succeeded.");
                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());

                req.HttpContext.Response.Headers.Add("CARD-ACTION-STATUS", "Responding to the survey was failed.");
                return new OkResult();
            }
        }

    }

}
