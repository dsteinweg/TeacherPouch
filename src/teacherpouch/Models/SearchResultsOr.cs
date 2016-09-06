using System;
using System.Collections.Generic;
using System.Linq;

namespace TeacherPouch.Models
{
    public class SearchResultsOr
    {
        public SearchResultsOr(string searchTerm)
        {
            SearchTerm = searchTerm;
        }

        public string SearchTerm { get; set; }
        public TimeSpan SearchDuration { get; set; }
        public List<TagSearchResult> TagResults { get; set; } = new List<TagSearchResult>();
        public bool HasAnyResults { get { return this.TagResults.Any(); } }
    }
}
