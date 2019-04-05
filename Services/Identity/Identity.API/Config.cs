using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XJeunot.PhysicalStoreApps.Services.Identity.API
{
    // TODO : OAuth2 List Users & Apps in code : For Test Only !!!!
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource()
                {
                    Name = "psa_api_customer",
                    DisplayName = "My API Customer into the PhysicalStoreApps",
                    ApiSecrets = new Collection<Secret>() { new Secret("fda96c0a-628f-44fa-90b0-04e0f08ab853") },
                    Scopes = new Collection<Scope> { new Scope("customer") }
                },
                new ApiResource()
                {
                    Name = "psa_api_store",
                    DisplayName = "My API Store into the PhysicalStoreApps",
                    ApiSecrets = new Collection<Secret>() { new Secret("2e1ace1f-ae52-4307-a888-3a7b13df2cba") },
                    Scopes = new Collection<Scope> { new Scope("store") }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = new Collection<string> { "password" },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "customer", "store" },
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false
                }
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>()
            {
                new TestUser()
                {
                    Username = "xjt",
                    Password = "xjtpass",
                    IsActive = true,
                    SubjectId = "1"
                }
            };
        }
    }
}
