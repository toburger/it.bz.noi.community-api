using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;

namespace it.bz.noi.community_api
{
    public class TokenService
    {
        private readonly IMemoryCache cache;
        private readonly Settings settings;
        private readonly IConfidentialClientApplication identityClientApp;

        public TokenService(IMemoryCache cache, Settings settings)
        {
            this.cache = cache;
            this.settings = settings;

            this.identityClientApp =
                ConfidentialClientApplicationBuilder.Create(settings.ClientId)
                    .WithClientSecret(settings.ClientSecret)
                    .WithAuthority(AzureCloudInstance.AzurePublic, settings.TenantId)
                    .Build();
        }

        async Task<AuthenticationResult> GetAccessToken()
        {
            var tokenClient = identityClientApp.AcquireTokenForClient(settings.Scopes);
            var authenticationResult = await tokenClient.ExecuteAsync();
            return authenticationResult;
        }

        public async Task<string> FetchToken()
        {
            if (cache.TryGetValue("NOI_ACCESS_TOKEN", out string token))
            {
                return token;
            }
            else
            {
                var tokenmodel = await this.GetAccessToken();
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(tokenmodel.ExpiresOn);
                cache.Set("NOI_ACCESS_TOKEN", tokenmodel.AccessToken, options);
                return tokenmodel.AccessToken;
            }
        }
    }
}
