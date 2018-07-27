using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssetTrackerIDP.User.Data.Enums.Claims
{

    public enum Groups
    {
        Management,

        Retail,

        WholesaleBanking,

        Reporting,

        Admin,

    }

    public static class CompanyGroups
    {
        public static IEnumerable<SelectListItem> GetGroups()
        {
            return new List<SelectListItem>
            {
                new SelectListItem {Text = "Management", Value = "Management"},
                new SelectListItem {Text = "Retail", Value = "Retail"},
                new SelectListItem {Text = "Wholesale Banking", Value = "WholesaleBanking"},
                new SelectListItem {Text = "Admin", Value = "Admin"},
                new SelectListItem {Text = "Reporting", Value = "Reporting"}
            };

        }
    }
}
