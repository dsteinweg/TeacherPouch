using System;
using System.Collections.Generic;
using System.Linq;

using TeacherPouch.Models;

namespace TeacherPouch.Repositories
{
    public interface IRepository
    {
        IEnumerable<Photo> GetAllPhotos(bool allowPrivate);
        IEnumerable<Tag> GetAllTags(bool allowPrivate);

        Photo FindPhoto(int id, bool allowPrivate);
        Photo FindPhoto(Guid uniqueID, bool allowPrivate);
        void SavePhoto(Photo photo, IEnumerable<string> tagNames = null);
        void DeletePhoto(Photo photo);

        Tag FindTag(int id, bool allowPrivate);
        Tag FindTag(string name, bool allowPrivate);
        void SaveTag(Tag tag);
        void DeleteTag(Tag tag);

        IEnumerable<Tag> GetTagsForPhoto(Photo photo, bool allowPrivate);
        IEnumerable<Photo> GetPhotosForTag(Tag tag, bool allowPrivate);
        IEnumerable<Question> GetQuestionsForPhoto(Photo photo);

        Question FindQuestion(int id);
        IEnumerable<Question> GetAllQuestions();
        void InsertQuestion(Question question);
        void UpdateQuestion(Question question);
        void DeleteQuestion(Question question);

        SearchResultsOr SearchOr(string query, bool allowPrivate);
        SearchResultsAnd SearchAnd(string query, bool allowPrivate);

        IEnumerable<string> TagAutocompleteSearch(string query, bool allowPrivate);
    }
}
