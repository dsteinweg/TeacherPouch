namespace TeacherPouch.Models;

public class TagSearchResult(Tag tag)
{
    public Tag Tag { get; set; } = tag;
    public List<Photo> Photos { get; set; } = tag.PhotoTags.Select(pt => pt.Photo).ToList();
}
