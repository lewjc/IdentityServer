using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AssetTrackerIDP.Data;
using AssetTrackerIDP.Models;
using AssetTrackerIDP.User.Data.Enums.Roles;
using AssetTrackerIDP.ViewModels;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AssetTrackerIDP.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ClaimManagementController : Controller
    {
        // GET: ClaimManagement
         

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;

        public ClaimManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserManagementController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        [HttpGet]
        [ValidateAntiForgeryToken]
        public IActionResult AddGroup(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroup(AddGroupViewModel model)
        {

            return Ok();
        }               
    }
}