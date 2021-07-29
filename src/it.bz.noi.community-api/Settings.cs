using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace it.bz.noi.community_api
{
    public class Settings
    {
        private readonly Uri baseUri;
        private readonly string clientId;
        private readonly string tenantId;
        private readonly string clientSecret;
        private readonly string[] scopes;

        private Settings()
        {
            baseUri = new Uri(Environment.GetEnvironmentVariable("SERVICE_URL")!);
            clientId = Environment.GetEnvironmentVariable("CLIENT_ID")!;
            tenantId = Environment.GetEnvironmentVariable("TENANT_ID")!;
            clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET")!;
            scopes = new[] { Environment.GetEnvironmentVariable("SERVICE_SCOPE")! };
        }

        public Uri BaseUri => baseUri;

        public string ClientId => clientId;

        public string TenantId => tenantId;

        public string ClientSecret => clientSecret;

        public string[] Scopes => scopes;

        public static Settings Initialize()
        {
            return new Settings();
        }
    }
}
