using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace it.bz.noi.community_api
{
    public class Startup
    {
        private readonly Settings settings;
        private readonly IConfidentialClientApplication identityClientApp;
        private static AuthenticationResult? authenticationResult;

        public Startup()
        {
            settings = Settings.Initialize();

            identityClientApp =
                ConfidentialClientApplicationBuilder.Create(settings.ClientId)
                    .WithClientSecret(settings.ClientSecret)
                    .WithAuthority(AzureCloudInstance.AzurePublic, settings.TenantId)
                    .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        private async Task<string> GetAccessToken()
        {
            if (authenticationResult == null || authenticationResult.ExpiresOn >= DateTimeOffset.Now)
            {
                authenticationResult = await identityClientApp.AcquireTokenForClient(settings.Scopes).ExecuteAsync();
            }
            return authenticationResult.AccessToken;
        }

        private async Task<HttpResponseMessage> MakeProxyCall(HttpRequestMessage request, string accessToken)
        {
            //var client = clientFactory.CreateClient("dynamics365");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            return await client.SendAsync(request);
        }

        public async Task ProxyRequest(HttpContext context)
        {
            var httpRequest = Helpers.CreateProxyHttpRequest(context, settings.ServiceUri);
            //context.Logger.LogLine($"Sending HttpRequestMessage: {httpRequest.ToString()}");
            string accessToken = await GetAccessToken();
            //context.Logger.LogLine($"Got Access Token: {accessToken.Substring(0, 20)}...");
            var httpResponse = await MakeProxyCall(httpRequest, accessToken);
            //context.Logger.LogLine($"Received HttpResponseMessage: {httpResponse.ToString()}");
            await Helpers.CopyProxyHttpResponse(context, httpResponse);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/{**path}", async context =>
                {
                    //await context.Response.WriteAsync($"Hello World {context.Request.Query}");
                    await ProxyRequest(context);
                });
            });
        }
    }
}
