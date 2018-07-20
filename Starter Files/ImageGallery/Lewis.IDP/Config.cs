using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lewis.IDP
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    // Info about the user, Sub must always be unique to the user.
                    SubjectId = "ABCDE-12345",
                    Username = "frank",
                    Password = "password",


                    // Represent info about the user. They are related to scopes.
                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Frank"),
                        new Claim("family_name", "Guy"),
                        new Claim("address", "Main road 1")

                    }

                },

                 new TestUser
                {
                    // Info about the user, Sub must always be unique to the user.
                    SubjectId = "ABCDE-54321",
                    Username = "Claire",
                    Password = "Password",


                    // Represent info about the user.
                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Frank"),
                        new Claim("family_name", "Guy"),
                        new Claim("address", "big road 2")


                    }

                 }
            };

        }

        // Identity resources map to scopes that give access to identity related information.
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                // Necessary when implementing oidc
                new IdentityResources.OpenId(),
                // Returns information about the profile of the user. This includes any extra claims we have defined
                // In the client object/database about the user.
                new IdentityResources.Profile(),
                new IdentityResources.Address(),

            };


        }

        public static IEnumerable<Client> GetClients()
        {

            return new List<Client>()
            {

                new Client
                {
                    ClientName = "Image Gallery",
                    ClientId = "imagegalleryclient",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    // This is where the IDP will redirect the client back to after sign in where the tokens are sent. 
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44304/signin-oidc"

                    },
                    PostLogoutRedirectUris= new List<string>
                    {
                        // Gives us a redirect when logging out of the client
                        "https://localhost:44304/signout-callback-oidc"

                    },
                    // These are the scopes that are allowed to be requested by the client, so by specifying certain scopes
                    // The application is allowed a certain level of information regarding the user.
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                    },
                    // Secret used to validate the client.
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }
            };
        }
    }

}
