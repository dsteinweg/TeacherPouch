using System;
using System.Collections.Generic;
using System.Linq;

namespace TeacherPouch.Models
{
    public class SearchResultsOr
    {
        public string SearchTerm { get; set; }
        public TimeSpan SearchDuration { get; set; }

        public List<TagSearchResult> TagResults { get; set; }

        public bool HasAnyResults { get { return this.TagResults.Any(); } }


        public SearchResultsOr(string searchTerm)
        {
            this.SearchTerm = searchTerm;

            this.TagResults = new List<TagSearchResult>();
        }
    }
}
