namespace TeacherPouch.Models;

public class SearchResultsAnd
{
    public SearchResultsAnd(string searchTerm)
    {
        SearchTerm = searchTerm;
    }

    public string SearchTerm { get; set; }
    public TimeSpan SearchDuration { get; set; }
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public List<Photo> Photos { get; set; } = new List<Photo>();
    public bool HasAnyResults => Photos.Any();
}
