using System;
using System.Globalization;
using System.Security.Principal;

using TeacherPouch.Providers;
using TeacherPouch.Utilities.Extensions;

namespace TeacherPouch.Web.ViewModels
{
    public class AdminViewModel
    {
        public string UserName { get; set; }
        public string Roles { get; set; }

        public AdminViewModel(IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                this.UserName = user.Identity.Name.ToTitleCase();

                var userRoles = SecurityHelper.GetRolesForUser(user);
                if (userRoles.SafeAny())
                    this.Roles = String.Join(", ", userRoles);
                else
                    this.Roles = "No roles found.";
            }
        }
    }
}
