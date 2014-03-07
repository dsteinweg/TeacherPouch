using System.Collections.Generic;

using TeacherPouch.Models;

namespace TeacherPouch.Web.ViewModels
{
    public class QuestionIndexViewModel
    {
        public IEnumerable<Question> Questions { get; set; }
        public bool DisplayAdminLinks { get; set; }
    }
}