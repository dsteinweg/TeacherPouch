using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class PhotoEditViewModel
    {
        public PhotoEditViewModel()
        {

        }

        public PhotoEditViewModel(Photo photo, string photoUrl, IEnumerable<Tag> tags)
        {
            Id = photo.Id;
            Name = photo.Name;
            PhotoUrl = photoUrl;
            Private = photo.IsPrivate;
            Tags = String.Join(", ", tags.Select(t => t.Name));
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public bool Private { get; set; }
        [DataType(DataType.MultilineText)]
        public string Tags { get; set; }
    }
}
