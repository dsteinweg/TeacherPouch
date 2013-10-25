using System.Collections.Generic;

namespace TeacherPouch.Models
{
    public class TagSearchResult
    {
        public Tag Tag { get; set; }
        public IEnumerable<Photo> Photos { get; set; }


        public TagSearchResult(Tag tag)
        {
            this.Tag = tag;
        }
    }
}
