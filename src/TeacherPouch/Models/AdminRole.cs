using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TeacherPouch.Models
{
    public class AdminRole : IdentityRole
    {
        public AdminRole() : base("Admin")
        {

        }
    }
}
