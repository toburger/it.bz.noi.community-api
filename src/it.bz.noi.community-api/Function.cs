using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.Identity.Client;
using System.Net.Http;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace it.bz.noi.community_api
{
    public class Functions
    {
        private readonly Settings settings;
        private readonly IConfidentialClientApplication identityClientApp;
        //private readonly IHttpClientFactory clientFactory;
        private AuthenticationResult? authenticationResult;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        //public Functions(IHttpClientFactory clientFactory)
        public Functions()
        {
            settings = Settings.Initialize();

            identityClientApp =
                ConfidentialClientApplicationBuilder.Create(settings.ClientId)
                    .WithClientSecret(settings.ClientSecret)
                    .WithAuthority(AzureCloudInstance.AzurePublic, settings.TenantId)
                    .Build();

            //this.clientFactory = clientFactory;
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

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The API Gateway response.</returns>
        public async Task<APIGatewayProxyResponse> Get(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine($"Get Request: {request.Path}?{request.QueryStringParameters} ({request.HttpMethod})\n");

            var httpRequest = Helpers.TransformFromAPIGatewayProxyRequest(settings.BaseUri, request);
            string accessToken = await GetAccessToken();
            var httpResponse = await MakeProxyCall(httpRequest, accessToken);
            var response = await Helpers.TransformToAPIGatewayResponse(httpResponse);
            return response;
        }
    }
}
