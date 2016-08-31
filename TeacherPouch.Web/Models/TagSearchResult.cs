using System.Collections.Generic;

namespace TeacherPouch.Models
{
    public class TagSearchResult
    {
        public TagSearchResult(Tag tag)
        {
            Tag = tag;
        }

        public Tag Tag { get; set; }
        public List<Photo> Photos { get; set; }
    }
}
