namespace TeacherPouch.Models;

public class SearchResultsAnd(string searchTerm)
{
    public string SearchTerm { get; set; } = searchTerm;
    public TimeSpan SearchDuration { get; set; }
    public List<Tag> Tags { get; set; } = [];
    public List<Photo> Photos { get; set; } = [];
    public bool HasAnyResults => Photos.Any();
}
