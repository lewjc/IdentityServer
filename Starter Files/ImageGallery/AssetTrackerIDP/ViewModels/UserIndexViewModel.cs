using AssetTrackerIDP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetTrackerIDP.ViewModels
{

    public class UserIndexViewModel
    {

        public IList<string> Errors { get; set; } = new List<string>();

       public IEnumerable<ApplicationUser> Users { get; private set; }
            = new List<ApplicationUser>();

        public UserIndexViewModel(List<ApplicationUser> users)
        {
            this.Users = users;
        }




    }
}
