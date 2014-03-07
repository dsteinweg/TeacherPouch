﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DbExtensions;

using TeacherPouch.Models;
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


        public IEnumerable<Photo> GetAllPhotos(bool allowPrivate)
        {
            var query = SQL.SELECT(PHOTO_COLUMN_NAMES)
                           .FROM(PHOTO_TABLE_NAME);

            if (!allowPrivate)
            {
                query = query.WHERE("IsPrivate = {0}", false);
            }

            List<Photo> photos = null;
            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                photos = connection.Map<Photo>(query).ToList();
            }

            return photos;
        }

        public IEnumerable<Tag> GetAllTags(bool allowPrivate)
        {
            var query = SQL.SELECT(TAG_COLUMN_NAMES)
                           .FROM(TAG_TABLE_NAME);

            if (!allowPrivate)
            {
                query = query.WHERE("IsPrivate = {0}", false);
            }

            List<Tag> tags = null;
            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                tags = connection.Map<Tag>(query).ToList();
            }

            return tags;
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

            using (var connection = ConnectionHelper.GetSQLiteConnection())
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

            using (var connection = ConnectionHelper.GetSQLiteConnection())
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

                using (var connection = ConnectionHelper.GetSQLiteConnection())
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

                using (var connection = ConnectionHelper.GetSQLiteConnection())
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

                using (var connection = ConnectionHelper.GetSQLiteConnection())
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

            using (var connection = ConnectionHelper.GetSQLiteConnection())
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

            using (var connection = ConnectionHelper.GetSQLiteConnection())
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

                    using (var connection = ConnectionHelper.GetSQLiteConnection())
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

            using (var connection = ConnectionHelper.GetSQLiteConnection())
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

                using (var connection = ConnectionHelper.GetSQLiteConnection())
                {
                    connection.Execute(query);
                }
            }

        }



        public IEnumerable<Tag> GetTagsForPhoto(Photo photo, bool allowPrivate)
        {
            var photoTagAssociations = PhotoTagAssociation.GetAll(photo).ToList();
            var tagIDsForPhoto = photoTagAssociations.Where(assoc => assoc.PhotoID == photo.ID).Select(assoc => assoc.TagID);

            return GetAllTags(allowPrivate).Where(tag => tagIDsForPhoto.Contains(tag.ID));
        }

        public IEnumerable<Photo> GetPhotosForTag(Tag tag, bool allowPrivate)
        {
            var photoTagAssociations = PhotoTagAssociation.GetAll(tag);
            var photoIDsForTag = photoTagAssociations.Where(assoc => assoc.TagID == tag.ID).Select(assoc => assoc.PhotoID);

            return GetAllPhotos(allowPrivate).Where(photo => photoIDsForTag.Contains(photo.ID));
        }



        public Question FindQuestion(int id)
        {
            var query = SQL.SELECT("*")
                           .FROM("Question")
                           .WHERE("ID = {0}", id);

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                return connection.Map<Question>(query).FirstOrDefault();
            }
        }

        public IEnumerable<Question> GetAllQuestions()
        {
            var query = SQL.SELECT("*")
                           .FROM("Question")
                           .ORDER_BY("ID");

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                return connection.Map<Question>(query).ToList();
            }
        }

        public IEnumerable<Question> GetQuestionsForPhoto(Photo photo)
        {
            var query = SQL.SELECT("*")
                           .FROM("Question")
                           .WHERE("PhotoID = {0}", photo.ID)
                           .ORDER_BY("\"Order\"");

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                return connection.Map<Question>(query).ToList();
            }
        }

        public void InsertQuestion(Question question)
        {
            if (question.ID != 0)
                throw new ApplicationException("Cannot insert a question that has an existing ID.");

            var insertQuestionQuery = SQL.INSERT_INTO("Question(\"PhotoID\", \"Text\", \"SentenceStarters\", \"Order\")")
                                        .VALUES(question.PhotoID, question.Text, question.SentenceStarters, question.Order);

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                connection.Open();

                connection.Execute(insertQuestionQuery);

                question.ID = (int)connection.LastInsertRowId;
            }
        }

        public void UpdateQuestion(Question question)
        {
            if (question.ID == 0)
                throw new ApplicationException("Cannot update question without an ID.");

            var updateQuestionQuery = SQL.UPDATE("Question")
                                             .SET("PhotoID = {0}", question.PhotoID)
                                             .SET("Text = {0}", question.Text)
                                             .SET("SentenceStarters = {0}", question.SentenceStarters)
                                             .SET("\"Order\" = {0}", question.Order)
                                             .WHERE("ID = {0}", question.ID);

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                connection.Execute(updateQuestionQuery);
            }
        }

        public void DeleteQuestion(Question question)
        {
            if (question.ID == 0)
                throw new ApplicationException("Cannot delete question without an ID.");

            var deleteQuestionQuery = SQL.DELETE_FROM("Question")
                                         .WHERE("ID = {0}", question.ID);

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                connection.Execute(deleteQuestionQuery);
            }
        }



        public SearchResultsOr SearchOr(string query, bool allowPrivate)
        {
            query = query.ToLower();

            var stopwatch = Stopwatch.StartNew();

            var results = new SearchResultsOr(query);

            var exactTagMatch = FindTag(query, allowPrivate);
            if (exactTagMatch != null)
            {
                var exactTagResult = new TagSearchResult(exactTagMatch);
                exactTagResult.Photos = GetPhotosForTag(exactTagMatch, allowPrivate).ToList();

                results.TagResults.Add(exactTagResult);
            }

            Tag pluralSuffixTagMatch = null;
            Tag singularSuffixTagMatch = null;
            if (!query.EndsWith("s"))
            {
                pluralSuffixTagMatch = FindTag(query + "s", allowPrivate);
                if (pluralSuffixTagMatch != null)
                {
                    results.TagResults.Add(PopulateTagSearchResult(pluralSuffixTagMatch, allowPrivate));
                }
            }
            else // ends with 's'
            {
                singularSuffixTagMatch = FindTag(query.TrimEnd('s'), allowPrivate);
                if (singularSuffixTagMatch != null)
                {
                    results.TagResults.Add(PopulateTagSearchResult(singularSuffixTagMatch, allowPrivate));
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
                foreach (var tag in startsWithTagMatches)
                {
                    results.TagResults.Add(PopulateTagSearchResult(tag, allowPrivate));
                }
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
                foreach (var tag in endsWithTagMatches)
                {
                    results.TagResults.Add(PopulateTagSearchResult(tag, allowPrivate));
                }
            }


            // If no results were found using exact tag and plural/singular searching, try splitting the search query into tokens and search for multiple tags.
            if (!results.HasAnyResults)
            {
                var searchWords = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var tagSearchResults = new List<TagSearchResult>(searchWords.Length);
                foreach (var searchWord in searchWords)
                {
                    var tag = FindTag(searchWord, allowPrivate);
                    if (tag != null)
                    {
                        results.TagResults.Add(PopulateTagSearchResult(tag, allowPrivate));
                    }
                }
            }

            stopwatch.Stop();

            results.SearchDuration = stopwatch.Elapsed;

            return results;
        }

        public SearchResultsAnd SearchAnd(string query, bool allowPrivate)
        {
            var stopwatch = Stopwatch.StartNew();

            var allTags = GetAllTags(allowPrivate);

            var searchWords = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var matchingTags = new List<Tag>();
            foreach (var searchWord in searchWords)
            {
                var tag = allTags.FirstOrDefault(t => t.Name.Equals(searchWord, StringComparison.OrdinalIgnoreCase));
                if (tag != null)
                    matchingTags.Add(tag);
            }

            IEnumerable<Photo> uniquePhotos = new List<Photo>();
            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                foreach (var matchingTag in matchingTags)
                {
                    var sqlQuery = SQL.SELECT("DISTINCT Photo.*")
                                      .FROM("Photo_Tag")
                                      .INNER_JOIN("Tag on Tag.ID = Photo_Tag.TagID")
                                      .INNER_JOIN("Photo on Photo.ID = Photo_Tag.PhotoID")
                                      .WHERE("Tag.ID = {0}", matchingTag.ID);

                    if (!allowPrivate)
                    {
                        sqlQuery = sqlQuery.WHERE("Photo.IsPrivate = {0}", false);
                    }

                    var matches = connection.Map<Photo>(sqlQuery);

                    if (!uniquePhotos.Any())
                    {
                        uniquePhotos = matches.ToList();
                    }
                    else
                    {
                        uniquePhotos = uniquePhotos.Intersect(matches.ToList());
                    }
                }

                uniquePhotos = uniquePhotos.Distinct();
            }

            var results = new SearchResultsAnd(query);
            results.Tags = matchingTags;
            results.Photos = uniquePhotos.ToList();

            stopwatch.Stop();

            results.SearchDuration = stopwatch.Elapsed;

            return results;
        }

        public IEnumerable<string> TagAutocompleteSearch(string query, bool allowPrivate)
        {
            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                var sqlQuery = SQL.SELECT("*")
                                  .FROM("Tag")
                                  .WHERE("Name LIKE {0}", (query + "%"));

                if (!allowPrivate)
                {
                    sqlQuery = sqlQuery.WHERE("IsPrivate = {0}", false);
                }

                sqlQuery = sqlQuery.ORDER_BY("Name");

                return connection.Map<Tag>(sqlQuery).ToList().Select(tag => tag.Name);
            }
        }

        private TagSearchResult PopulateTagSearchResult(Tag tag, bool allowPrivate)
        {
            var tagResult = new TagSearchResult(tag);
            tagResult.Photos = GetPhotosForTag(tag, allowPrivate).ToList();

            return tagResult;
        }
    }
}
