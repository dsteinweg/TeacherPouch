using System.ComponentModel.DataAnnotations;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels;

public class PhotoEditViewModel
{
    public PhotoEditViewModel()
    {

    }

    public PhotoEditViewModel(Photo photo, string photoUrl, IEnumerable<Tag> tags)
    {
        Id = photo.Id;
        Name = photo.Name;
        PhotoUrl = photoUrl;
        Private = photo.IsPrivate;
        Tags = string.Join(", ", tags.Select(t => t.Name));
    }

    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    public string? PhotoUrl { get; set; }
    public bool Private { get; set; }
    [DataType(DataType.MultilineText)]
    public string? Tags { get; set; }
}
