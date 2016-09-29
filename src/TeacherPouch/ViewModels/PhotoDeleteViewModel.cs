using System.Collections.Generic;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class PhotoDeleteViewModel
    {
        public PhotoDeleteViewModel(Photo photo, IEnumerable<Tag> tags)
        {
            Photo = photo;
            Tags = tags;
        }

        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public Photo Photo { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}
