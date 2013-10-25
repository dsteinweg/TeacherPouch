using System.Linq;
using System.Collections.Generic;

namespace TeacherPouch.Models
{
    public class SearchResults
    {
        public string SearchTerm { get; set; }

        public TagSearchResult ExactTagResult { get; set; }
        public TagSearchResult PluralTagResult { get; set; }
        public TagSearchResult SingularTagResult { get; set; }
        public IEnumerable<TagSearchResult> EndsWithTagResults { get; set; }
        public IEnumerable<TagSearchResult> StartsWithTagResults { get; set; }

        public bool HasAnyResults
        {
            get
            {
                return (
                    this.ExactTagResult != null
                 || this.PluralTagResult != null
                 || this.SingularTagResult != null
                 || (this.StartsWithTagResults != null && this.StartsWithTagResults.Any())
                 || (this.EndsWithTagResults != null && this.EndsWithTagResults.Any())
                );
            }
        }

        public SearchResults(string searchTerm)
        {
            this.SearchTerm = searchTerm;
        }
    }
}
