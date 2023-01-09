using Microsoft.AspNetCore.Identity;

namespace TeacherPouch.ViewModels;

public class AdminViewModel
{
    public AdminViewModel(IdentityUser user, IEnumerable<string> roles)
    {
        UserName = user.UserName;

        if (roles.Any())
            Roles = string.Join(", ", roles);
        else
            Roles = "No roles found.";
    }

    public string? UserName { get; }
    public string Roles { get; }
}
