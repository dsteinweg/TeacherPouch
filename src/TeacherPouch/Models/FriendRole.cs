using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TeacherPouch.Models
{
    public class FriendRole : IdentityRole
    {
        public FriendRole() : base("Friend")
        {

        }
    }
}
