using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AssetTrackerIDP.User.Data.Enums.Roles;
using AssetTrackerIDP.User.Data.Enums.Claims;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AssetTrackerIDP.ViewModels
{
    public class AddUserViewModel
    {

        [Required]
        [StringLength(50, ErrorMessage = "Username Must have atleast one character ", MinimumLength =1)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage ="The name must have atleast one character", MinimumLength =1)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The name must have atleast one character", MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [EnumDataType(typeof(Permission))]
        public Permission UserRole{ get; set; }
    
        public IList<SelectListItem> AvailableGroups { get; set; }
        
        [Required]
        public IList<string> SelectedGroups{ get; set; }

        [Required]
        [Display(Name = "IsLocked")]
        public bool IsLocked { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Windows Domain")]
        public string WindowsDomain{ get; set; }


        [DataType(DataType.Text)]
        [Display(Name = "Windows Name")]
        public string WindowsName { get; set; }



        [Required]
        [Display(Name = "UserImage")]
        public IFormFile UserImage { get; set; }



    }

}

