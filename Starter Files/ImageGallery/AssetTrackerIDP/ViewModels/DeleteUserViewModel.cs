using AssetTrackerIDP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AssetTrackerIDP.ViewModels
{

    public class DeleteUserViewModel
    {
        [Required]
        [Display(Name ="UserToDelete")]
       public ApplicationUser UserToDelete { get; set; }

    }
}
