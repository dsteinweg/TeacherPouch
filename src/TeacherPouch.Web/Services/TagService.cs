using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using TeacherPouch.Data;
using TeacherPouch.Models;

namespace TeacherPouch.Services
{
    public class TagService
    {
        public TagService(
            IHttpContextAccessor httpContextAccessor,
            TeacherPouchDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = dbContext;
        }

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TeacherPouchDbContext _db;

        public IEnumerable<Tag> GetAllTags()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return _db.Tags.ToList();

            return _db.Tags.Where(tag => !tag.IsPrivate).ToList();
        }

        public Tag FindTag(int id)
        {
            var tag = _db.Tags.FirstOrDefault(t => t.Id == id);
            if (tag == null)
                return null;

            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && tag.IsPrivate)
                return null;

            return tag;
        }

        public Tag FindTag(string name)
        {
            var tag = _db.Tags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (tag == null)
                return null;

            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && tag.IsPrivate)
                return null;

            return tag;
        }

        public IEnumerable<Tag> FindTagsLike(string name)
        {
            var tags = _db.Tags.Where(t => t.Name.Contains(name));

            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return tags.Where(tag => !tag.IsPrivate);

            return tags;
        }

        public void SaveTag(Tag tag)
        {
            if (tag.Id == default(int))
                _db.Tags.Add(tag);
            else
                _db.Update(tag);

            _db.SaveChanges();
        }

        public void DeleteTag(Tag tag)
        {
            _db.Remove(tag);
            _db.SaveChanges();
        }
    }
}
