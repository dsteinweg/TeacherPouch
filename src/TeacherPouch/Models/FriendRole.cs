using Microsoft.AspNetCore.Identity;

namespace TeacherPouch.Models;

public class FriendRole : IdentityRole
{
    public FriendRole() : base("Friend")
    {

    }
}
