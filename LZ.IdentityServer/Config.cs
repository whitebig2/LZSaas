using Duende.IdentityServer.Models;

namespace LZ.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource("CompanyService", "公司服务")
                {
                    Scopes = { "scope1" }
                }
            };
        }
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
                {
                    new ApiScope(name:"api",displayName:"MyApi")
                };

        public static IEnumerable<Client> Clients =>
            new Client[]
                {
                    new Client{
                ClientId ="client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                ClientSecrets = {
                new Secret("secret".Sha256())
                },
                AllowAccessTokensViaBrowser = true,
                AllowedScopes = {"api"}
                    }
                };
    }
}