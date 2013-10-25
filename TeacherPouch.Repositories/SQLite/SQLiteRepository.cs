using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using DbExtensions;

using TeacherPouch.Models;
using TeacherPouch.Utilities.Caching;
using TeacherPouch.Utilities.Extensions;

namespace TeacherPouch.Repositories.SQLite
{
    public class SQLiteRepository : IRepository
    {
        private const string PHOTO_TABLE_NAME = "Photo";
        private const string PHOTO_COLUMN_NAMES = "ID, Name, UniqueID, IsPrivate";

        private const string TAG_TABLE_NAME = "Tag";
        private const string TAG_COLUMN_NAMES = "ID, Name, IsPrivate";

        private const string PHOTOTAG_TABLE_NAME = "Photo_Tag";
        private const string PHOTOTAG_COLUMN_NAMES = "TagID, PhotoID";


        public IQueryable<Photo> GetAllPhotos(bool allowPrivate)
        {
            var query = SQL.SELECT(PHOTO_COLUMN_NAMES)
                           .FROM(PHOTO_TABLE_NAME);

            if (!allowPrivate)
            {
                query = query.WHERE("IsPrivate = {0}", false);
            }

            List<Photo> photos = null;
            using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
            {
                photos = connection.Map<Photo>(query).ToList();
            }

            return photos.AsQueryable();
        }

        public IQueryable<Tag> GetAllTags(bool allowPrivate)
        {
            var query = SQL.SELECT(TAG_COLUMN_NAMES)
                           .FROM(TAG_TABLE_NAME);

            if (!allowPrivate)
            {
                query = query.WHERE("IsPrivate = {0}", false);
            }

            List<Tag> tags = null;
            using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
            {
                tags = connection.Map<Tag>(query).ToList();
            }

            return tags.AsQueryable();
        }



        public Photo FindPhoto(int id, bool allowPrivate)
        {
            var query = SQL.SELECT(PHOTO_COLUMN_NAMES)
                           .FROM(PHOTO_TABLE_NAME)
                           .WHERE("ID = {0}", id);

            if (!allowPrivate)
            {
                query = query.WHERE("IsPrivate = {0}", false);
            }

            using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
            {
                return connection.Map<Photo>(query).FirstOrDefault();
            }
        }

        public Photo FindPhoto(Guid uniqueID, bool allowPrivate)
        {
            var query = SQL.SELECT(PHOTO_COLUMN_NAMES)
                           .FROM(PHOTO_TABLE_NAME)
                           .WHERE("UniqueID = {0}", uniqueID);

            if (!allowPrivate)
            {
                query = query.WHERE("IsPrivate = {0}", false);
            }

            using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
            {
                return connection.Map<Photo>(query).FirstOrDefault();
            }
        }

        public void SavePhoto(Photo photo, IEnumerable<string> tagNames = null)
        {
            if (photo.ID == 0)
            {
                // Save the photo.
                var insertPhotoQuery = SQL.INSERT_INTO(String.Format("{0}({1})", PHOTO_TABLE_NAME, "Name, UniqueID, IsPrivate"))
                                          .VALUES(photo.Name, photo.UniqueID, photo.IsPrivate);

                using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
                {
                    connection.Execute(insertPhotoQuery);
                }

                // Retrieve the photo to get its ID.
                var retrievedPhoto = FindPhoto(photo.UniqueID, true);

                if (retrievedPhoto == null)
                    throw new ApplicationException("Unable to retrieve photo that was just saved.");

                photo.ID = retrievedPhoto.ID;
            }
            else
            {
                // Update the photo name.
                var updateQuery = SQL.UPDATE(PHOTO_TABLE_NAME)
                                     .SET("Name = {0}", photo.Name)
                                     .SET("IsPrivate = {0}", photo.IsPrivate)
                                     .WHERE("ID = {0}", photo.ID);

                using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
                {
                    connection.Execute(updateQuery);
                }
            }

            // Delete all existing tag associations for the photo; we'll add them back below.
            PhotoTagAssociation.DeleteTagAssociationsForPhoto(photo);

            if (tagNames.SafeAny())
            {
                var photoTags = new List<Tag>();

                // Create new tags.
                foreach (var tagName in tagNames)
                {
                    var existingTag = FindTag(tagName, true);
                    if (existingTag == null)
                    {
                        var newTag = new Tag()
                        {
                            Name = tagName,
                            IsPrivate = false
                        };

                        SaveTag(newTag);

                        photoTags.Add(newTag);
                    }
                    else
                    {
                        photoTags.Add(existingTag);
                    }
                }

                // Associate the photo to all tags.
                foreach (var tag in photoTags)
                {
                    PhotoTagAssociation.EnsurePhotoTagAssociation(photo, tag);
                }
            }
        }

        public void DeletePhoto(Photo photo)
        {
            if (photo.ID != 0)
            {
                PhotoTagAssociation.DeleteTagAssociationsForPhoto(photo);

                var deleteQuery = SQL.DELETE_FROM(PHOTO_TABLE_NAME)
                                     .WHERE("ID = {0}", photo.ID);

                using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
                {
                    connection.Execute(deleteQuery);
                }
            }
        }



        public Tag FindTag(int id, bool allowPrivate)
        {
            var query = SQL.SELECT(TAG_COLUMN_NAMES)
                           .FROM(TAG_TABLE_NAME)
                           .WHERE("ID = {0}", id);

            if (!allowPrivate)
            {
                query = query.WHERE("IsPrivate = {0}", false);
            }

            using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
            {
                return connection.Map<Tag>(query).FirstOrDefault();
            }
        }

        public Tag FindTag(string name, bool allowPrivate)
        {
            var query = SQL.SELECT(TAG_COLUMN_NAMES)
                           .FROM(TAG_TABLE_NAME)
                           .WHERE("Name = {0}", name);

            if (!allowPrivate)
            {
                query = query.WHERE("IsPrivate = {0}", false);
            }

            using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
            {
                return connection.Map<Tag>(query).FirstOrDefault();
            }
        }

        public void SaveTag(Tag tag)
        {
            if (tag.ID == 0)
            {
                var existingTag = FindTag(tag.Name, true);
                if (existingTag == null)
                {
                    var insertQuery = SQL.INSERT_INTO(String.Format("{0}({1})", TAG_TABLE_NAME, "Name, IsPrivate"))
                                         .VALUES(tag.Name, tag.IsPrivate);

                    using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
                    {
                        connection.Execute(insertQuery);
                    }

                    var retrievedTag = FindTag(tag.Name, true);
                    tag.ID = retrievedTag.ID;
                }
                else
                {
                    tag.ID = existingTag.ID;

                    UpdateTag(tag);
                }
            }
            else
            {
                UpdateTag(tag);
            }
        }

        private void UpdateTag(Tag tag)
        {
            var updateQuery = SQL.UPDATE(TAG_TABLE_NAME)
                                 .SET("Name = {0}", tag.Name)
                                 .SET("IsPrivate = {0}", tag.IsPrivate)
                                 .WHERE("ID = {0}", tag.ID);

            using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
            {
                connection.Execute(updateQuery);
            }
        }

        public void DeleteTag(Tag tag)
        {
            if (tag.ID != 0)
            {
                PhotoTagAssociation.DeletePhotoAssociationsForTag(tag);

                var query = SQL.DELETE_FROM(TAG_TABLE_NAME)
                               .WHERE("ID = {0}", tag.ID);

                using (var connection = new SQLiteConnection(ConnectionStringHelper.GetConnectionString()))
                {
                    connection.Execute(query);
                }
            }

        }



        public IQueryable<Tag> GetTagsForPhoto(Photo photo, bool allowPrivate)
        {
            var photoTagAssociations = PhotoTagAssociation.GetAll(photo).ToList();
            var tagIDsForPhoto = photoTagAssociations.Where(assoc => assoc.PhotoID == photo.ID).Select(assoc => assoc.TagID);

            return GetAllTags(allowPrivate).Where(tag => tagIDsForPhoto.Contains(tag.ID));
        }

        public IQueryable<Photo> GetPhotosForTag(Tag tag, bool allowPrivate)
        {
            var photoTagAssociations = PhotoTagAssociation.GetAll(tag);
            var photoIDsForTag = photoTagAssociations.Where(assoc => assoc.TagID == tag.ID).Select(assoc => assoc.PhotoID);

            return GetAllPhotos(allowPrivate).Where(photo => photoIDsForTag.Contains(photo.ID));
        }



        public SearchResults Search(string query, bool allowPrivate)
        {
            var results = new SearchResults(query);

            if (String.IsNullOrWhiteSpace(query))
                return results;

            var exactTagMatch = FindTag(query, allowPrivate);
            if (exactTagMatch != null)
            {
                var exactTagResult = new TagSearchResult(exactTagMatch);
                exactTagResult.Photos = GetPhotosForTag(exactTagMatch, allowPrivate);

                results.ExactTagResult = exactTagResult;
            }

            Tag pluralSuffixTagMatch = null;
            Tag singularSuffixTagMatch = null;
            if (!query.EndsWith("s"))
            {
                pluralSuffixTagMatch = FindTag(query + "s", allowPrivate);
                if (pluralSuffixTagMatch != null)
                {
                    var pluralSearchResult = new TagSearchResult(pluralSuffixTagMatch);
                    pluralSearchResult.Photos = GetPhotosForTag(pluralSuffixTagMatch, allowPrivate);

                    results.PluralTagResult = pluralSearchResult;
                }
            }
            else // ends with 's'
            {
                singularSuffixTagMatch = FindTag(query.TrimEnd('s'), allowPrivate);
                if (singularSuffixTagMatch != null)
                {
                    var singularSearchResult = new TagSearchResult(singularSuffixTagMatch);
                    singularSearchResult.Photos = GetPhotosForTag(singularSuffixTagMatch, allowPrivate);

                    results.SingularTagResult = singularSearchResult;
                }
            }

            var allTags = GetAllTags(allowPrivate);

            var startsWithTagMatches = from tag in allTags
                                       where (exactTagMatch == null || exactTagMatch.ID != tag.ID)
                                          && (pluralSuffixTagMatch == null || pluralSuffixTagMatch.ID != tag.ID)
                                          && (singularSuffixTagMatch == null || singularSuffixTagMatch.ID != tag.ID)
                                          && tag.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase)
                                       orderby tag.Name ascending
                                       select tag;

            if (startsWithTagMatches.SafeAny())
            {
                var startsWithTagResults = new List<TagSearchResult>(startsWithTagMatches.Count());
                foreach (var tag in startsWithTagMatches)
                {
                    var tagSearchResult = new TagSearchResult(tag);
                    tagSearchResult.Photos = GetPhotosForTag(tag, allowPrivate);

                    startsWithTagResults.Add(tagSearchResult);
                }

                results.StartsWithTagResults = startsWithTagResults;
            }

            var endsWithTagMatches = from tag in allTags
                                       where (exactTagMatch == null || exactTagMatch.ID != tag.ID)
                                          && (pluralSuffixTagMatch == null || pluralSuffixTagMatch.ID != tag.ID)
                                          && (singularSuffixTagMatch == null || singularSuffixTagMatch.ID != tag.ID)
                                          && tag.Name.EndsWith(query, StringComparison.OrdinalIgnoreCase)
                                       orderby tag.Name ascending
                                       select tag;

            if (endsWithTagMatches.SafeAny())
            {
                var endsWithTagResults = new List<TagSearchResult>(endsWithTagMatches.Count());
                foreach (var tag in endsWithTagMatches)
                {
                    var tagSearchResult = new TagSearchResult(tag);
                    tagSearchResult.Photos = GetPhotosForTag(tag, allowPrivate);

                    endsWithTagResults.Add(tagSearchResult);
                }

                results.EndsWithTagResults = endsWithTagResults;
            }

            return results;
        }
    }
}
