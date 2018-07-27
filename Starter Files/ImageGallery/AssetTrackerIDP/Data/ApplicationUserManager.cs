using AssetTrackerIDP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AssetTrackerIDP.Data
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }


        public async Task<IdentityResult> DeleteByIDAsync(string id)
        {
            var userToRemove = await base.FindByIdAsync(id);

            if (userToRemove == null)
            {
                // User doesnt exist

                return null;
            }
            await base.DeleteAsync(userToRemove);

            return IdentityResult.Success;

        }

        public async Task<IdentityResult> RemoveClaimsByTypeAsync(ApplicationUser user, string claimType)
        {
            IList<Claim> currentClaims = await base.GetClaimsAsync(user);

            if(currentClaims.Count == 0)
            {
                // none found by that name.
                return null;
            }

            // Filter by the claim name.
            var filteredClaims = currentClaims.Where(c => c.Type == claimType);

            await base.RemoveClaimsAsync(user, filteredClaims);

            return IdentityResult.Success;
        }


    }
   
}
