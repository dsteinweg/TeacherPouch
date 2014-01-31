using System;
using System.Collections.Generic;
using System.Linq;

namespace TeacherPouch.Models
{
    public class SearchResultsAnd
    {
        public string SearchTerm { get; set; }
        public TimeSpan SearchDuration { get; set; }

        public List<Tag> Tags { get; set; }
        public List<Photo> Photos { get; set; }

        public bool HasAnyResults { get { return this.Photos.Any(); } }

        public SearchResultsAnd(string searchTerm)
        {
            this.SearchTerm = searchTerm;

            this.Tags = new List<Tag>();
            this.Photos = new List<Photo>();
        }
    }
}
