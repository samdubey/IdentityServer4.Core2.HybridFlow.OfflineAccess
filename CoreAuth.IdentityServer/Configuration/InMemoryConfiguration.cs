using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAuth.Configuration
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[] {
                new ApiResource("socialnetwork", "Social Network")
            };
        }

        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new IdentityResource[] {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[] {
                new Client
                {
                    ClientId = "socialnetwork",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "socialnetwork" }
                },
                new Client
                {
                    ClientId="socialnetwork_implicit",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = new [] {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "socialnetwork" },
                    AllowAccessTokensViaBrowser= true,
                    RedirectUris=new[]{ "http://localhost:33000/signin-oidc" },
                    PostLogoutRedirectUris={ "http://localhost:33000/signout-callback-oidc" }
                }
            };
        }

        public static IEnumerable<TestUser> Users()
        {
            return new[] {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "mail@filipekberg.se",
                    Password = "password"
                }
            };
        }
    }
}
