using System.Collections.Generic;
using System.Linq;

namespace TeacherPouch.Models
{
    public class SearchResults
    {
        public string SearchTerm { get; set; }
        public SearchOperator Operator { get; set; }
        public List<TagSearchResult> TagResults { get; set; }

        public List<Tag> Tags { get; set; }
        public List<Photo> PhotoResultsFromAndedTags { get; set; }

        public bool HasAnyResults { get { return (this.TagResults.Any() || this.PhotoResultsFromAndedTags.Any()); } }

        public SearchResults(string searchTerm, SearchOperator searchOp)
        {
            this.SearchTerm = searchTerm;
            this.Operator = searchOp;

            this.TagResults = new List<TagSearchResult>();

            this.Tags = new List<Tag>();
            this.PhotoResultsFromAndedTags = new List<Photo>();
        }
    }
}
