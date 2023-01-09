using System.ComponentModel.DataAnnotations;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels;

public class QuestionCreateViewModel
{
    public QuestionCreateViewModel()
    {

    }

    public QuestionCreateViewModel(Photo photo)
    {
        Photo = photo;
        Tags = photo.PhotoTags.Select(photoTag => photoTag.Tag);
    }

    public Photo Photo { get; set; } = new Photo();
    public IEnumerable<Tag> Tags { get; set; } = Enumerable.Empty<Tag>();

    [Display(Name = "Question")]
    [Required]
    [DataType(DataType.MultilineText)]
    public string? QuestionText { get; set; }

    [Display(Name = "Sentence Starters")]
    [DataType(DataType.MultilineText)]
    public string? QuestionSentenceStarters { get; set; }

    [Display(Name = "Question Order (optional)")]
    [Range(1, int.MaxValue, ErrorMessage = "Order must be a whole number greater than 0.")]
    public int QuestionOrder { get; set; }
}
