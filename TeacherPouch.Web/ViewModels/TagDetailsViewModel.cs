using System.Collections.Generic;
using System.Linq;
using TeacherPouch.Data;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class TagDetailsViewModel
    {
        public TagDetailsViewModel(IRepository repository, Tag tag, bool allowPrivate)
        {
            Tag = tag;
            TaggedPhotos = repository.GetPhotosForTag(tag, allowPrivate).ToList();
        }

        public Tag Tag { get; set; }
        public List<Photo> TaggedPhotos { get; set; }
    }
}
