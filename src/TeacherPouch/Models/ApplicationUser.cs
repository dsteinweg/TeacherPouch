using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TeacherPouch.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Password { get; set; }
    }
}