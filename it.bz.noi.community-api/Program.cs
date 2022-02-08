using System.Net.Http.Headers;
using it.bz.noi.community_api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

dotenv.net.DotEnv.Load();

var settings = Settings.Initialize();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<Settings>(settings);
builder.Services.AddSingleton<TokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtBearerOptions =>
{
    jwtBearerOptions.Authority = settings.OpenIdAuthority;
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("noi-auth", policy =>
        policy.RequireAuthenticatedUser());
});

var routes = new[]
{
    new RouteConfig()
    {
        RouteId = "route1",
        ClusterId = "cluster1",
        AuthorizationPolicy = "noi-auth",
        Match = new RouteMatch()
        {
            Path = "{**catch-all}"
        }
    }
};
var clusters = new[]
{
    new ClusterConfig()
    {
        ClusterId = "cluster1",
        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { "destination1", new DestinationConfig() { Address = settings.ServiceUri } }
        }
    }
};

builder.Services
    .AddReverseProxy()
    .LoadFromMemory(routes, clusters)
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async transformContext =>
        {
            var tokenService = builderContext.Services.GetService<TokenService>()!;
            var accessToken = await tokenService.FetchToken();
            var authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            transformContext.ProxyRequest.Headers.Authorization = authorization; 
        });
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();
