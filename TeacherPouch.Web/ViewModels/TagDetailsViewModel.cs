using System.Collections.Generic;
using System.Linq;

using TeacherPouch.Models;
using TeacherPouch.Repositories;

namespace TeacherPouch.Web.ViewModels
{
    public class TagDetailsViewModel
    {
        public Tag Tag { get; set; }
        public List<Photo> TaggedPhotos { get; set; }

        public TagDetailsViewModel(IRepository repository, Tag tag, bool allowPrivate)
        {
            this.Tag = tag;
            this.TaggedPhotos = repository.GetPhotosForTag(tag, allowPrivate).ToList();
        }
    }
}