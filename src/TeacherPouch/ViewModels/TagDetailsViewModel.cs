using System.Collections.Generic;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class TagDetailsViewModel
    {
        public TagDetailsViewModel(Tag tag, IEnumerable<Photo> photos)
        {
            Tag = tag;
            Photos = photos;
        }

        public Tag Tag { get; }
        public IEnumerable<Photo> Photos { get; }
    }
}
