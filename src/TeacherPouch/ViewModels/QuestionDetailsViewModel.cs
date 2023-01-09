using System.ComponentModel.DataAnnotations;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels;

public class QuestionDetailsViewModel : IValidatableObject
{
    public QuestionDetailsViewModel()
    {

    }

    public QuestionDetailsViewModel(Question question, Photo photo)
    {
        QuestionId = question.Id;
        QuestionText = question.Text;
        QuestionSentenceStarters = !string.IsNullOrWhiteSpace(question.SentenceStarters) ? question.SentenceStarters : "(None)";
        QuestionOrder = (question.Order.HasValue) ? question.Order.Value.ToString() : "(None)";
        Photo = photo;
        Tags = photo.PhotoTags.Select(pt => pt.Tag);
    }

    public Photo? Photo { get; set; }
    public IEnumerable<Tag> Tags { get; } = Enumerable.Empty<Tag>();
    public int QuestionId { get; set; }
    [Display(Name = "Question")]
    public string? QuestionText { get; set; }
    [Display(Name = "Sentence Starters")]
    public string? QuestionSentenceStarters { get; set; }
    [Display(Name = "Question Order (optional)")]
    public string? QuestionOrder { get; set; }
    public bool DisplayAdminLinks { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Photo is null || Photo.Id <= 0)
            results.Add(new ValidationResult("Missing photo ID."));

        if (QuestionId <= 0)
            results.Add(new ValidationResult("Missing question ID."));

        if (string.IsNullOrWhiteSpace(QuestionText))
            results.Add(new ValidationResult("Question text cannot be empty."));

        if (!string.IsNullOrWhiteSpace(QuestionOrder) && !uint.TryParse(QuestionOrder, out var _))
            results.Add(new ValidationResult("Question order must be a whole number."));

        return results;
    }
}
