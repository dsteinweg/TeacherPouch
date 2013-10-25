using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Security;

namespace TeacherPouch.Providers
{
    public static class SecurityHelper
    {
        public static IEnumerable<string> GetRolesForUser(IPrincipal user)
        {
            return Roles.GetRolesForUser(user.Identity.Name);
        }

        public static bool UserIsAdmin(IPrincipal user)
        {
            return user.IsInRole(TeacherPouchRoles.Admin);
        }

        public static bool UserCanSeePrivateRecords(IPrincipal user)
        {
            return ( user.IsInRole(TeacherPouchRoles.Admin) || user.IsInRole(TeacherPouchRoles.Friend) );
        }
    }
}
