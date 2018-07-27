using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AssetTrackerIDP.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //
        // Summary:
        //     Gets or sets the First Name for this user.
        public string FirstName { get; set; }
        //
        // Summary:
        //     Gets or sets the Last Name for this user.
        public string LastName { get; set; }

        // Summary:
        //      Whether or not the account is locked
        public bool IsLocked { get; set; }

        // Summary:
        //     Byte array holding the user image.
        public byte[] UserImage { get; set; }


    }
}
