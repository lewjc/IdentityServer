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
                    Username = "Frank",
                    Password = "Password",


                    // Represent info about the user. They are related to scopes.
                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Frank"),
                        new Claim("family_name", "Guy"),

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

            };

            
        }

        public static IEnumerable<Client> GetClients()
        {

            return new List<Client>();
        }


    }
}
