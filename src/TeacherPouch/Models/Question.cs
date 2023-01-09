using System.ComponentModel.DataAnnotations;

namespace TeacherPouch.Models;

public class Question : IValidatableObject
{
    public int Id { get; set; }
    public int PhotoId { get; set; }
    public string Text { get; set; } = default!;
    public string? SentenceStarters { get; set; }
    public int? Order { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (PhotoId == default)
            results.Add(new ValidationResult("Question needs to be associated to a photo."));

        if (string.IsNullOrWhiteSpace(Text))
            results.Add(new ValidationResult("Question text cannot be blank."));

        return results;
    }

    public override string ToString() => $"{Id} - {Text}";
}
