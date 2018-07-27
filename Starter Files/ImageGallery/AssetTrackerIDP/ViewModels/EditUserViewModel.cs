using AssetTrackerIDP.User.Data.Enums.Roles;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AssetTrackerIDP.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [StringLength(50, ErrorMessage = "Username Must have atleast one character ", MinimumLength = 1)]
        [Display(Name = "Username")]
        public string Username { get; set; }
        
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The name must have atleast one character", MinimumLength = 1)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The name must have atleast one character", MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EnumDataType(typeof(Permission))]
        public Permission UserRole { get; set; }

        
        public IList<SelectListItem> AvailableGroups { get; set; }

        // Use this variable so we know what groups the user was in before, and is now in after the model post.
        public IList<string> PreviousGroups { get; set; }

        [Required]
        public IList<string> SelectedGroups { get; set; }

 
        [Display(Name = "IsLocked")]
        public bool IsLocked { get; set; }

        [Display(Name = "User Image")]
        public IFormFile UserImage { get; set; }



    }
}
