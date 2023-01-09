using TeacherPouch.Models;

namespace TeacherPouch.ViewModels;

public class PhotoDeleteViewModel
{
    public PhotoDeleteViewModel(Photo photo, string photoUrl, IEnumerable<Tag> tags)
    {
        Id = photo.Id;
        Name = photo.Name;
        PhotoUrl = photoUrl;
        Tags = tags;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string PhotoUrl { get; set; }
    public IEnumerable<Tag> Tags { get; set; }
}
