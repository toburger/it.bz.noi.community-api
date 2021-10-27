using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace it.bz.noi.community_api
{
    [ApiController]
    [Route("")]
    public class ProxyController : Controller
    {
        private static readonly Settings settings;
        private static readonly IConfidentialClientApplication identityClientApp;

        static ProxyController()
        {
            settings = Settings.Initialize();

            identityClientApp =
                ConfidentialClientApplicationBuilder.Create(settings.ClientId)
                    .WithClientSecret(settings.ClientSecret)
                    .WithAuthority(AzureCloudInstance.AzurePublic, settings.TenantId)
                    .Build();
        }

        private static async Task<string> GetAccessToken()
        {
            var tokenClient = identityClientApp.AcquireTokenForClient(settings.Scopes);
            var authenticationResult = await tokenClient.ExecuteAsync();
            return authenticationResult.AccessToken;
        }

        private static async Task<HttpResponseMessage> MakeProxyCall(HttpRequestMessage request, string accessToken)
        {
            //var client = clientFactory.CreateClient("dynamics365");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            return await client.SendAsync(request);
        }

        [Authorize]
        public async Task GetAsync()
        {
            // Remove the Authorization Header as it is useless (and wrong) for the rest of the request.
            HttpContext.Request.Headers.Remove("Authorization");

            var httpRequest = Helpers.CreateProxyHttpRequest(HttpContext, settings.ServiceUri);
            //context.Logger.LogLine($"Sending HttpRequestMessage: {httpRequest.ToString()}");
            string accessToken = await GetAccessToken();
            //context.Logger.LogLine($"Got Access Token: {accessToken.Substring(0, 20)}...");
            var httpResponse = await MakeProxyCall(httpRequest, accessToken);
            //context.Logger.LogLine($"Received HttpResponseMessage: {httpResponse.ToString()}");
            await Helpers.CopyProxyHttpResponse(HttpContext, httpResponse);
        }
    }
}
