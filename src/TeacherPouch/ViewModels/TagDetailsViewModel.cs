using System.Collections.Generic;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class TagDetailsViewModel
    {
        public TagDetailsViewModel(Tag tag, IEnumerable<Photo> photos)
        {
            Tag = tag;
            TagId = tag.Id;
            TagName = tag.Name;
            IsPrivateHtml = tag.IsPrivate ? "<strong>Yes</strong>" : "<span>No</span>";
            Photos = photos;
        }

        public Tag Tag { get; }
        public int TagId { get; }
        public string TagName { get; }
        public string IsPrivateHtml { get; }
        public IEnumerable<Photo> Photos { get; }
    }
}
