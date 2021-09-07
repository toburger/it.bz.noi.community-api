using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace it.bz.noi.community_api
{
    public static class Proxy
    {
        private static readonly Settings settings;
        private static readonly IConfidentialClientApplication identityClientApp;
        private static AuthenticationResult? authenticationResult;

        static Proxy()
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
            if (authenticationResult == null || authenticationResult.ExpiresOn >= DateTimeOffset.Now)
            {
                authenticationResult = await identityClientApp.AcquireTokenForClient(settings.Scopes).ExecuteAsync();
            }
            return authenticationResult.AccessToken;
        }

        private static async Task<HttpResponseMessage> MakeProxyCall(HttpRequestMessage request, string accessToken)
        {
            //var client = clientFactory.CreateClient("dynamics365");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            return await client.SendAsync(request);
        }

        public static async Task MakeProxyRequest(HttpContext context)
        {
            var httpRequest = Helpers.CreateProxyHttpRequest(context, settings.ServiceUri);
            //context.Logger.LogLine($"Sending HttpRequestMessage: {httpRequest.ToString()}");
            string accessToken = await GetAccessToken();
            //context.Logger.LogLine($"Got Access Token: {accessToken.Substring(0, 20)}...");
            var httpResponse = await MakeProxyCall(httpRequest, accessToken);
            //context.Logger.LogLine($"Received HttpResponseMessage: {httpResponse.ToString()}");
            await Helpers.CopyProxyHttpResponse(context, httpResponse);
        }

    }
}
