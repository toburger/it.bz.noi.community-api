using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

using it.bz.noi.community_api;
using dotenv.net;

namespace it.bz.noi.community_api.Tests
{
    public class FunctionTest
    {
        public FunctionTest()
        {
            DotEnv.Load();
        }

        [Fact]
        public void TestRequestUriConstruction()
        {
            Uri baseUri = new("https://dummy/api");
            var request = new APIGatewayProxyRequest() { Path = "/hello" };
            var uri = Helpers.ConstructRequestUri(baseUri, request);
            Assert.Equal(new Uri("https://dummy/api/hello"), uri);
        }

        [Fact(Skip = "Integration Test")]
        public async Task TestGetMethod()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            Functions functions = new Functions();

            request = new APIGatewayProxyRequest();
            context = new TestLambdaContext();
            response = await functions.Get(request, context);
            Assert.Equal(200, response.StatusCode);
            Assert.NotNull(response.Body);
        }
    }
}
