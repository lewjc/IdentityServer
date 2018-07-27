using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetTrackerIDP.Data
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        { }

        public ApplicationRole(string name) : base(name)
        { }
        
        public string Description { get; set; }
   

    }
}
