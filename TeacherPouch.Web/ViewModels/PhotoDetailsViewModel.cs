using System;
using System.Collections.Generic;
using System.Linq;

using TeacherPouch.Models;
using TeacherPouch.Repositories;
using TeacherPouch.Utilities;

namespace TeacherPouch.Web.ViewModels
{
    public class PhotoDetailsViewModel
    {
        public Photo Photo { get; set; }
        public string SmallFileSize { get; set; }
        public string LargeFileSize { get; set; }
        public List<Tag> PhotoTags { get; set; }
        public Tag SearchResultTag { get; set; }
        public Photo PreviousPhoto { get; set; }
        public Photo NextPhoto { get; set; }

        public PhotoDetailsViewModel(IRepository repository, Photo photo, bool allowPrivate, string tagName = null)
        {
            this.Photo = photo;

            this.SmallFileSize = PhotoHelper.GetPhotoFileSize(photo, PhotoSizes.Small);
            this.LargeFileSize = PhotoHelper.GetPhotoFileSize(photo, PhotoSizes.Large);

            if (!String.IsNullOrWhiteSpace(tagName))
            {
                this.SearchResultTag = repository.FindTag(tagName, allowPrivate);

                if (this.SearchResultTag != null)
                {
                    var photosForTag = repository.GetPhotosForTag(this.SearchResultTag, allowPrivate).ToList();
                    var photoIndexInPhotosList = photosForTag.IndexOf(this.Photo);

                    this.PreviousPhoto = photosForTag.ElementAtOrDefault(photoIndexInPhotosList - 1);
                    if (this.PreviousPhoto == null && photosForTag.Count > 1)
                        this.PreviousPhoto = photosForTag.ElementAtOrDefault(photosForTag.Count - 1);

                    this.NextPhoto = photosForTag.ElementAtOrDefault(photoIndexInPhotosList + 1);
                    if (this.NextPhoto == null && photosForTag.Count > 1)
                        this.NextPhoto = photosForTag.ElementAtOrDefault(0);
                }
            }

            this.PhotoTags = repository.GetTagsForPhoto(photo, allowPrivate).ToList();
        }
    }
}
