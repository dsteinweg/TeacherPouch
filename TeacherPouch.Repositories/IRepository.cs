using System;
using System.Collections.Generic;
using System.Linq;

using TeacherPouch.Models;

namespace TeacherPouch.Repositories
{
    public interface IRepository
    {
        IQueryable<Photo> GetAllPhotos(bool allowPrivate);
        IQueryable<Tag> GetAllTags(bool allowPrivate);

        Photo FindPhoto(int id, bool allowPrivate);
        Photo FindPhoto(Guid uniqueID, bool allowPrivate);
        void SavePhoto(Photo photo, IEnumerable<string> tagNames = null);
        void DeletePhoto(Photo photo);

        Tag FindTag(int id, bool allowPrivate);
        Tag FindTag(string name, bool allowPrivate);
        void SaveTag(Tag tag);
        void DeleteTag(Tag tag);

        IQueryable<Tag> GetTagsForPhoto(Photo photo, bool allowPrivate);
        IQueryable<Photo> GetPhotosForTag(Tag tag, bool allowPrivate);

        SearchResults Search(string query, bool allowPrivate);
    }
}
