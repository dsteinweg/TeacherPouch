namespace TeacherPouch.Models;

public class TagSearchResult
{
    public TagSearchResult(Tag tag)
    {
        Tag = tag;
        Photos = tag.PhotoTags.Select(pt => pt.Photo).ToList();
    }

    public Tag Tag { get; set; }
    public List<Photo> Photos { get; set; }
}
