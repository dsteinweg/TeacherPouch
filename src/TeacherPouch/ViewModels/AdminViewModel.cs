using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TeacherPouch.ViewModels
{
    public class AdminViewModel
    {
        public AdminViewModel(IdentityUser user, IEnumerable<string> roles)
        {
            UserName = user.UserName;

            if (roles.Any())
                Roles = String.Join(", ", roles);
            else
                Roles = "No roles found.";
        }

        public string UserName { get; }
        public string Roles { get; }
    }
}
