using TeacherPouch.Models;

namespace TeacherPouch.ViewModels;

public class PhotoDeleteViewModel(Photo photo, string photoUrl, IEnumerable<Tag> tags)
{
    public int Id { get; set; } = photo.Id;
    public string Name { get; set; } = photo.Name;
    public string PhotoUrl { get; set; } = photoUrl;
    public IEnumerable<Tag> Tags { get; set; } = tags;
}
