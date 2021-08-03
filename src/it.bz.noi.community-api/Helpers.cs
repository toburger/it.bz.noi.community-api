using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace it.bz.noi.community_api
{
    public class Helpers
    {
        public static Uri ConstructRequestUri(Uri serviceUri, APIGatewayProxyRequest request)
        {
            string relativeUri = request.Path == null ? "/" : $"{request.Path}";
            string queryString = request.QueryStringParameters?.Count > 0 ? $"?{string.Join("&", request.QueryStringParameters.Select(x => $"{x.Key}={x.Value}"))}" : "";
            return new Uri($"{serviceUri}{relativeUri}{queryString}");
        }

        public static HttpRequestMessage TransformFromAPIGatewayProxyRequest(Uri serviceUri, APIGatewayProxyRequest request)
        {
            var method = request.HttpMethod == null ? HttpMethod.Get : new HttpMethod(request.HttpMethod);
            var uri = ConstructRequestUri(serviceUri, request);
            var content = request.Body != null ? new StringContent(request.Body) : null;
            var httpRequest = new HttpRequestMessage(method, uri) { Content = content };
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    httpRequest.Headers.Add(header.Key, header.Value);
                }
            }
            CleanupHostHeader(httpRequest, uri.Host);
            return httpRequest;
        }

        /// <summary>
        /// Fix the host, as the Dynamics 365 service returns a 404 otherwise.
        /// </summary>
        private static void CleanupHostHeader(HttpRequestMessage httpRequest, string host)
        {
            if (httpRequest.Headers.Contains("Host"))
            {
                httpRequest.Headers.Remove("Host");
            }
            httpRequest.Headers.Add("Host", host);
        }

        /// <summary>
        /// Check the response size as AWS Lambda has a limit
        /// of 6MB and throws an error if the size is bigger
        /// than the maximum size.
        /// </summary>
        private static bool CheckResponseSize(HttpResponseMessage httpResponse, [NotNullWhen(true)] out APIGatewayProxyResponse? response)
        {
            if (httpResponse.Content.Headers.ContentLength >= 6291456)
            {
                response = new APIGatewayProxyResponse
                {
                    StatusCode = 413
                };
                return true;
            }
            response = null;
            return false;
        }

        public static async Task<APIGatewayProxyResponse> TransformToAPIGatewayResponse(HttpResponseMessage httpResponse)
        {
            if (CheckResponseSize(httpResponse, out var response))
            {
                return response;
            }
                
            string body = await httpResponse.Content.ReadAsStringAsync();
            var headers = httpResponse.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)httpResponse.StatusCode,
                Body = body,
                Headers = headers
            };
        }
    }
}

