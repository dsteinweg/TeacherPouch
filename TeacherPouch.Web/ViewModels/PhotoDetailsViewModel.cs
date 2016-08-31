using System;
using System.Collections.Generic;
using System.Linq;
using TeacherPouch.Data;
using TeacherPouch.Helpers;
using TeacherPouch.Models;

namespace TeacherPouch.ViewModels
{
    public class PhotoDetailsViewModel
    {
        public PhotoDetailsViewModel(IRepository repository, Photo photo, bool allowPrivate, bool showAdminLinks, string tagName = null, string tag2Name = null)
        {
            Photo = photo;

            SmallFileSize = PhotoHelper.GetPhotoFileSize(photo, PhotoSizes.Small);
            LargeFileSize = PhotoHelper.GetPhotoFileSize(photo, PhotoSizes.Large);

            Questions = repository.GetQuestionsForPhoto(photo);

            ShowAdminLinks = showAdminLinks;

            if (!String.IsNullOrWhiteSpace(tagName))
            {
                SearchResultTag = repository.FindTag(tagName, allowPrivate);

                if (SearchResultTag != null)
                {
                    var photosForTag1 = repository.GetPhotosForTag(SearchResultTag, allowPrivate);

                    IEnumerable<Photo> photosForTag2 = null;
                    if (!String.IsNullOrWhiteSpace(tag2Name))
                    {
                        SearchResultTag2 = repository.FindTag(tag2Name, allowPrivate);

                        if (SearchResultTag2 != null)
                        {
                            photosForTag2 = repository.GetPhotosForTag(SearchResultTag2, allowPrivate);
                        }
                    }

                    var allPhotos = photosForTag2.Any() ?
                        photosForTag1.Intersect(photosForTag2).Distinct().ToList() :
                        photosForTag1.ToList();

                    var photoIndexInPhotosList = allPhotos.IndexOf(Photo);

                    PreviousPhoto = allPhotos.ElementAtOrDefault(photoIndexInPhotosList - 1);
                    if (PreviousPhoto == null && allPhotos.Count() > 1)
                        PreviousPhoto = allPhotos.ElementAtOrDefault(allPhotos.Count() - 1);

                    NextPhoto = allPhotos.ElementAtOrDefault(photoIndexInPhotosList + 1);
                    if (NextPhoto == null && allPhotos.Count() > 1)
                        NextPhoto = allPhotos.ElementAtOrDefault(0);
                }
            }

            PhotoTags = repository.GetTagsForPhoto(photo, allowPrivate).ToList();
        }

        public Photo Photo { get; set; }
        public string SmallFileSize { get; set; }
        public string LargeFileSize { get; set; }
        public List<Tag> PhotoTags { get; set; }
        public Tag SearchResultTag { get; set; }
        public Tag SearchResultTag2 { get; set; }
        public Photo PreviousPhoto { get; set; }
        public Photo NextPhoto { get; set; }
        public IEnumerable<Question> Questions { get; set; }
        public bool ShowAdminLinks { get; set; }
    }
}
