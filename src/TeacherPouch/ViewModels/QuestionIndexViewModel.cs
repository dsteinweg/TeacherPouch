using TeacherPouch.Models;

namespace TeacherPouch.ViewModels;

public class QuestionIndexViewModel
{
    public IEnumerable<Question> Questions { get; set; } = Enumerable.Empty<Question>();
    public bool DisplayAdminLinks { get; set; }
}
