using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Collections.Generic;
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
            return new Uri(serviceUri + relativeUri);
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
            return httpRequest;
        }
        public static async Task<APIGatewayProxyResponse> TransformToAPIGatewayResponse(HttpResponseMessage httpResponse)
        {
            string body = await httpResponse.Content.ReadAsStringAsync();
            var headers = httpResponse.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());
            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)httpResponse.StatusCode,
                Body = body,
                Headers = headers
            };
            return response;
        }
    }
}

