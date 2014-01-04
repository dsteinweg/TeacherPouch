using System;
using System.Collections.Generic;
using System.Linq;

using TeacherPouch.Models;
using TeacherPouch.Repositories;
using TeacherPouch.Utilities;
using TeacherPouch.Utilities.Extensions;

namespace TeacherPouch.Web.ViewModels
{
    public class PhotoDetailsViewModel
    {
        public Photo Photo { get; set; }
        public string SmallFileSize { get; set; }
        public string LargeFileSize { get; set; }
        public List<Tag> PhotoTags { get; set; }
        public Tag SearchResultTag { get; set; }
        public Tag SearchResultTag2 { get; set; }
        public Photo PreviousPhoto { get; set; }
        public Photo NextPhoto { get; set; }

        public PhotoDetailsViewModel(IRepository repository, Photo photo, bool allowPrivate, string tagName = null, string tag2Name = null)
        {
            this.Photo = photo;

            this.SmallFileSize = PhotoHelper.GetPhotoFileSize(photo, PhotoSizes.Small);
            this.LargeFileSize = PhotoHelper.GetPhotoFileSize(photo, PhotoSizes.Large);

            if (!String.IsNullOrWhiteSpace(tagName))
            {
                this.SearchResultTag = repository.FindTag(tagName, allowPrivate);

                if (this.SearchResultTag != null)
                {
                    var photosForTag1 = repository.GetPhotosForTag(this.SearchResultTag, allowPrivate);

                    IQueryable<Photo> photosForTag2 = null;
                    if (!String.IsNullOrWhiteSpace(tag2Name))
                    {
                        this.SearchResultTag2 = repository.FindTag(tag2Name, allowPrivate);

                        if (this.SearchResultTag2 != null)
                        {
                            photosForTag2 = repository.GetPhotosForTag(this.SearchResultTag2, allowPrivate);
                        }
                    }

                    var allPhotos = photosForTag2.SafeAny() ?
                        photosForTag1.Intersect(photosForTag2).Distinct().ToList() :
                        photosForTag1.ToList();

                    var photoIndexInPhotosList = allPhotos.IndexOf(this.Photo);

                    this.PreviousPhoto = allPhotos.ElementAtOrDefault(photoIndexInPhotosList - 1);
                    if (this.PreviousPhoto == null && allPhotos.Count > 1)
                        this.PreviousPhoto = allPhotos.ElementAtOrDefault(allPhotos.Count - 1);

                    this.NextPhoto = allPhotos.ElementAtOrDefault(photoIndexInPhotosList + 1);
                    if (this.NextPhoto == null && allPhotos.Count > 1)
                        this.NextPhoto = allPhotos.ElementAtOrDefault(0);
                }
            }

            this.PhotoTags = repository.GetTagsForPhoto(photo, allowPrivate).ToList();
        }
    }
}
