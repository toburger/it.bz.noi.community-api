using it.bz.noi.community_api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

dotenv.net.DotEnv.Load();

var settings = Settings.Initialize();

var identityClientApp =
    ConfidentialClientApplicationBuilder.Create(settings.ClientId)
        .WithClientSecret(settings.ClientSecret)
        .WithAuthority(AzureCloudInstance.AzurePublic, settings.TenantId)
        .Build();

async Task<string> GetAccessToken()
{
    var tokenClient = identityClientApp.AcquireTokenForClient(settings.Scopes);
    var authenticationResult = await tokenClient.ExecuteAsync();
    return authenticationResult.AccessToken;
}

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async transformContext => {
            var accessToken = await GetAccessToken();
            transformContext.ProxyRequest.Headers.Remove("Authorization");
            transformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
        });
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();
