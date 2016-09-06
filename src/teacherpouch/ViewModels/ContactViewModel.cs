using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class ContactViewModel
    {
        public string ErrorMessage { get; set; }
        public ContactSubmission Submission { get; set; } = new ContactSubmission();
    }
}
