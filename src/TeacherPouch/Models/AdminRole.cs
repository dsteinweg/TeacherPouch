using Microsoft.AspNetCore.Identity;

namespace TeacherPouch.Models;

public class AdminRole : IdentityRole
{
    public AdminRole() : base("Admin")
    {

    }
}
