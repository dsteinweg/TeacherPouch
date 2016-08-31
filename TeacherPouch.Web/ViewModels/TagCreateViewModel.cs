using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class TagCreateViewModel
    {
        public Tag Tag { get; set; } = new Tag();
        public string ErrorMessage { get; set; }
    }
}
