using it.bz.noi.community_api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async transformContext => {
            var tokenService = builderContext.Services.GetService<TokenService>()!;
            var accessToken = await tokenService.FetchToken();
            transformContext.ProxyRequest.Headers.Remove("Authorization");
            transformContext.ProxyRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
        });
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();
