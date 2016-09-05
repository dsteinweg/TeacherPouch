using System.Collections.Generic;
using System.Linq;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class TagDetailsViewModel
    {
        public TagDetailsViewModel(Tag tag)
        {
            Tag = tag;
            TaggedPhotos = tag.PhotoTags.Select(photoTag => photoTag.Photo).ToList();
        }

        public Tag Tag { get; set; }
        public List<Photo> TaggedPhotos { get; set; }
    }
}
