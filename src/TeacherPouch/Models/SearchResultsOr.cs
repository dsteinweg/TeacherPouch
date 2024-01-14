namespace TeacherPouch.Models;

public class SearchResultsOr(string searchTerm)
{
    public string SearchTerm { get; set; } = searchTerm;
    public TimeSpan SearchDuration { get; set; }
    public List<TagSearchResult> TagResults { get; set; } = [];
    public bool HasAnyResults { get { return this.TagResults.Any(); } }
}
