using TeacherPouch.Models;

namespace TeacherPouch.ViewModels;

public class TagDetailsViewModel(Tag tag, IEnumerable<Photo> photos)
{
    public Tag Tag { get; } = tag;
    public int TagId { get; } = tag.Id;
    public string TagName { get; } = tag.Name;
    public string IsPrivateHtml { get; } = tag.IsPrivate ? "<strong>Yes</strong>" : "<span>No</span>";
    public IEnumerable<Photo> Photos { get; } = photos;
}
