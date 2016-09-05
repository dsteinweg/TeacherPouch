using System.Collections.Generic;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class PhotoDetailsViewModel
    {
        public PhotoDetailsViewModel(
            Photo photo,
            string photoUrl,
            string smallFileSize,
            string largeFileSize,
            Tag searchResultTag,
            Tag searchResultTag2,
            Photo previousPhoto,
            Photo nextPhoto,
            bool userIsAdmin)
        {
            Photo = photo;
            PhotoUrl = photoUrl;
            SmallFileSize = smallFileSize;
            LargeFileSize = largeFileSize;
            SearchResultTag = searchResultTag;
            SearchResultTag2 = searchResultTag2;
            PreviousPhoto = previousPhoto;
            NextPhoto = nextPhoto;
            ShowAdminLinks = userIsAdmin;
        }

        public Photo Photo { get; set; }
        public string PhotoUrl { get; set; }
        public string SmallFileSize { get; set; }
        public string LargeFileSize { get; set; }
        public Tag SearchResultTag { get; set; }
        public Tag SearchResultTag2 { get; set; }
        public Photo PreviousPhoto { get; set; }
        public Photo NextPhoto { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public bool ShowAdminLinks { get; set; }
    }
}
