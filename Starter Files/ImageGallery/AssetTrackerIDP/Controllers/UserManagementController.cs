using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AssetTrackerIDP.Data;
using AssetTrackerIDP.Models;
using AssetTrackerIDP.User.Data.Enums.Claims;
using AssetTrackerIDP.User.Data.Enums.Roles;
using AssetTrackerIDP.ViewModels;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Logging;
using static AssetTrackerIDP.Utility.Guard;

namespace AssetTrackerIDP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {

        private readonly ApplicationUserManager _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;

        public UserManagementController(
            ApplicationUserManager userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserManagementController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {



                // Get all of our users and pass through to the view.
              List<ApplicationUser> users = _userManager.Users.ToList();

            UserIndexViewModel vm = new UserIndexViewModel(users);

            return View(vm);
        }

        [HttpGet]
        public IActionResult AddUser(string returnUrl = null)
        {
            // Initialise groups
            var vm = new AddUserViewModel
            {
                AvailableGroups = CompanyGroups.GetGroups().ToList()
            };

            ViewData["ReturnUrl"] = returnUrl;
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(AddUserViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsLocked = model.IsLocked

                };

                // Copy the image to a stream and save it as a byte array.
                if (!IsNull(model.UserImage))
                {
                    user.UserImage = await ImageToBitArray(model.UserImage);
                }
                var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
                new ClaimsPrincipal(id);

             


                // Add this user to our store.
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // If the user has been added to some groups, we must set them as claims.
                    if (model.SelectedGroups.Count > 0)
                    {
                        foreach (string group in model.SelectedGroups)
                        {
                            await _userManager.AddClaimAsync(user, new Claim("Group", group));

                        }

                    }

                    // Here we add windows as a default login provider.
                    if (true)
                    {
                        await _userManager.AddLoginAsync(user, new UserLoginInfo(AccountOptions.WindowsAuthenticationSchemeName, "headquarters//lewis" ,"lewis"));
                    }


                    await _userManager.AddToRoleAsync(user, model.UserRole.ToString());
                    _logger.LogInformation("{0} created a new account with password.", User.Identity.Name);

                    // IF YOU WANT TO USE EMAIL VERIFICATION OR ANY OTHER POST-REGISTRATION STUFF, DO IT HERE.
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    // Return to wherever the user was after logging on.
                    return RedirectToLocal(returnUrl);

                }
                AddErrors(result);
            }

            return AddUser();

        }

        [Authorize]
        public async Task<FileContentResult> GetProfilePictureAsync(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            // User doesn't exist or photo doesn't exist, return a stock photo.
            if (user == null || user.UserImage == null)
            {
                // Convert the stock image into a file.
                string stockImagePath = @"wwwroot\7LayerLogoNoBg.png";
                byte[] imageData = null;
                var fileInfo = new FileInfo(stockImagePath);
                var path = Path.Combine(Environment.CurrentDirectory, stockImagePath);
                Debug.WriteLine(Path.Combine(Environment.CurrentDirectory, stockImagePath));
                long imageFileLength = fileInfo.Length;
                var fileStream = new FileStream(stockImagePath, FileMode.Open, FileAccess.Read);
                var binaryReader = new BinaryReader(fileStream);

                imageData = binaryReader.ReadBytes((int)imageFileLength);

                return File(imageData, "image/png");
            }

            // User's profile picture.
            return new FileContentResult(user.UserImage, "image/png");


        }



        [HttpGet]
        public async Task<IActionResult> DeleteUser(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            var vm = new DeleteUserViewModel
            {
                UserToDelete = user
            };


            if (IsNull(user))
            {
                var ErrorIndexVm = BuildIndexErrorViewModel(new List<string> { "User does not exist" });
                return View("Index", ErrorIndexVm);
            }


            //ViewData["ReturnUrl"] = returnUrl;

            return View("DeleteUser", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id, string buttonDelete)
        {
            if (ModelState.IsValid)
            {
                if (buttonDelete == "Yes")
                {

                    await _userManager.DeleteByIDAsync(id);
                }

                return RedirectToAction("Index");
            }

            // If we get this far, the user ID was invalid.
            return View("Index", BuildIndexErrorViewModel(new List<string>() { "User was invalid" }));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            if (IsNull(user))
            {
                var ErrorIndexVm = BuildIndexErrorViewModel(new List<string> { "User does not exist" });
                return View("Index", ErrorIndexVm);
            }

            var vm = await BuildEditUserViewModel(user);

            return View(vm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model, string buttonSubmit)
        {

            if (buttonSubmit == "Submit")
            {
                if (ModelState.IsValid)
                {
                    var userToUpdate = await _userManager.FindByIdAsync(model.Id);

                    // img
                    if (!IsNull(model.UserImage))
                    {
                        userToUpdate.UserImage = await ImageToBitArray(model.UserImage);
                    }

                    // details
                    userToUpdate.FirstName = model.FirstName ?? userToUpdate.FirstName;
                    userToUpdate.LastName = model.LastName ?? userToUpdate.LastName;
                    userToUpdate.IsLocked = model.IsLocked;


                    // casting to hashset to check if the claims passed for editing are the same as the ones the user had before the update request.
                    var previousClaimValues = new HashSet<string>(model.PreviousGroups ?? new List<string>());

                    // If there were no previous groups, we just add them in.
                    if(previousClaimValues.Count == 0)
                    {
                        await _userManager.AddClaimsAsync(userToUpdate, CreateClaimList(model.SelectedGroups, "Group"));

                    }
                    else if(!previousClaimValues.SetEquals(model.SelectedGroups))
                    {
                        // we have a change so must redo user group claims.
                        IdentityResult result = await _userManager.RemoveClaimsByTypeAsync(userToUpdate, "Group");

                        if (result.Succeeded)
                        {
                            var newClaims = CreateClaimList(model.SelectedGroups, "Group");
                            await _userManager.AddClaimsAsync(userToUpdate, newClaims);
                        }
                    }

                    Permission newRole = model.UserRole;
                    Permission currentRole = await getCurrentPermissionAsync(userToUpdate);

                    if (!(currentRole == newRole))
                    {
                        // if we get here, the previous role is changed and the new role is updated.
                        await _userManager.RemoveFromRoleAsync(userToUpdate, currentRole.ToString());
                        await _userManager.AddToRoleAsync(userToUpdate, newRole.ToString());
                    }


                    await _userManager.UpdateAsync(userToUpdate);
                    return RedirectToAction("Index");
                }

                // There were errors
                return await EditUser(model.Id);


            } // User pressed cancel
            else
            {
                return RedirectToAction("Index");
            }



        }



        #region HelperMethods


        private async Task<EditUserViewModel> BuildEditUserViewModel(ApplicationUser user)
        {
            IList<string> Roles = await _userManager.GetRolesAsync(user);

            Roles = Roles.Where(r => (r == Permission.User.ToString()) || (r == Permission.Admin.ToString())).ToList();

            Permission currentRole = Permission.User;
            // The user doesn't have a role, shouldn't be possible but set them to default user anyways.
            if (Roles.Count == 0)
            {
                currentRole = Permission.User;
            }
            // Must check what permission the user currently has.
            else
            {
                var permissionValues = Enum.GetNames(typeof(Permission));

                foreach (var role in Roles)
                {
                    // Convert the role stored in the database into a type of permission enum.
                    if (permissionValues.Contains(role))
                    {
                        currentRole = (Permission)Enum.Parse(typeof(Permission), role);
                    }
                }
            }

            // Need to get the users' groups from the claims table.
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);


            // Filter claims to just groups the user belongs to?
            IList<string> groups = userClaims.Where(c => c.Type == "Group").Select(c => c.Value).ToList();


            // Construct the model, what the user will be able to see and modify.

            var vm = new EditUserViewModel()
            {
                Id = user.Id,
                PreviousGroups = new List<string>(),
                AvailableGroups = CompanyGroups.GetGroups().ToList(),
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsLocked = user.IsLocked,
                UserRole = currentRole,
                SelectedGroups = groups,

            };


            return vm;

        }


        /// <summary>
        /// Used when errors have arisen with views that return to the View Users index
        /// </summary>
        /// <param name="errors">Errors that occured on a view</param>
        /// <returns>View model populated with errors.</returns>
        private UserIndexViewModel BuildIndexErrorViewModel(IList<string> errors)
        {
            var vm = new UserIndexViewModel(_userManager.Users.ToList())
            {
                Errors = errors
            };

            return vm;


        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

        }

        private async Task<Byte[]> ImageToBitArray(IFormFile file)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch
            {
                return new byte[0];
            }
            
        }

        /// <summary>
        /// Converts a list of values into a list of claims
        /// </summary>
        /// <param name="claimValues">List of values</param>
        /// <param name="type">The type of claim to convert this list to </param>
        /// <returns></returns>
        private IEnumerable<Claim> CreateClaimList(IList<string> claimValues, string type)
        {
            foreach(var value in claimValues)
            {
                yield return new Claim(type, value);
            }


        }

        private async Task<Permission> getCurrentPermissionAsync(ApplicationUser user)
        {
            var userRoles= await _userManager.GetRolesAsync(user);

            var permissionValues = Enum.GetNames(typeof(Permission));

            // Loop through all user roles
            foreach (var role in userRoles)
            {
                // If the user is susbscribed to a role located in our enum
                if (permissionValues.Contains(role))
                {
                    var currentRole = (Permission)Enum.Parse(typeof(Permission), role);
                    return currentRole;
                  
                }
            }

            // if the method doesn't find a role, we will return the basic user. This is because a user MUST have a role.
            return Permission.User;
        }


        #endregion


    }
}