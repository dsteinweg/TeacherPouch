using System.Collections.Generic;
using System.Linq;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class PhotoDetailsViewModel
    {
        public PhotoDetailsViewModel(
            Photo photo,
            string photoUrl,
            string smallFileSize,
            string smallDownloadUrl,
            string largeFileSize,
            string largeDownloadUrl,
            Tag searchResultTag,
            Tag searchResultTag2,
            Photo previousPhoto,
            Photo nextPhoto,
            bool userIsAdmin)
        {
            Photo = photo;
            PhotoUrl = photoUrl;
            Tags = photo.PhotoTags.Select(pt => pt.Tag);
            Questions = photo.Questions;
            SmallFileSize = smallFileSize;
            SmallDownloadUrl = smallDownloadUrl;
            LargeFileSize = largeFileSize;
            LargeDownloadUrl = largeDownloadUrl;
            SearchResultTag = searchResultTag;
            SearchResultTag2 = searchResultTag2;
            PreviousPhoto = previousPhoto;
            NextPhoto = nextPhoto;
            ShowAdminLinks = userIsAdmin;
        }

        public Photo Photo { get; }
        public string PhotoUrl { get; }
        public IEnumerable<Tag> Tags { get; }
        public IEnumerable<Question> Questions { get; }
        public string SmallFileSize { get; }
        public string SmallDownloadUrl { get; }
        public string LargeFileSize { get; }
        public string LargeDownloadUrl { get; }
        public Tag SearchResultTag { get; }
        public Tag SearchResultTag2 { get; }
        public Photo PreviousPhoto { get; }
        public Photo NextPhoto { get; }
        public bool ShowAdminLinks { get; }
    }
}
