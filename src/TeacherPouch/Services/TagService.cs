using Microsoft.EntityFrameworkCore;
using TeacherPouch.Data;
using TeacherPouch.Models;

namespace TeacherPouch.Services;

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

    public async Task<Tag[]> GetAllTags(CancellationToken cancellationToken = default)
    {
        if ((_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated).GetValueOrDefault())
            return await _db.Tags.ToArrayAsync(cancellationToken);

        return await _db.Tags
            .Where(tag => !tag.IsPrivate)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Tag?> FindTag(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _db.Tags
            .Where(t => t.Id == id)
            .Include(t => t.PhotoTags)
            .ThenInclude(pt => pt.Photo)
            .FirstOrDefaultAsync(cancellationToken);

        if (tag is null)
            return null;

        if (!(_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated).GetValueOrDefault() && tag.IsPrivate)
            return null;

        return tag;
    }

    public async Task<Tag?> FindTag(string name, CancellationToken cancellationToken = default)
    {
        var tags = await _db.Tags
            .Where(t => t.Name == name)
            .OrderBy(t => t.Id)
            .Include(t => t.PhotoTags)
            .ThenInclude(pt => pt.Photo)
            .ToArrayAsync(cancellationToken);

        var tag = tags.FirstOrDefault();

        if (tag is null)
            return null;

        if (!(_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated).GetValueOrDefault())
        {
            if (tag.IsPrivate)
                return null;

            tag.PhotoTags = tag.PhotoTags.Where(pt => !pt.Photo.IsPrivate).ToList();
        }

        return tag;
    }

    public async Task<Tag[]> FindTagsLike(string name, CancellationToken cancellationToken = default)
    {
        var tags = await _db.Tags.Where(t => t.Name.Contains(name)).ToArrayAsync(cancellationToken);

        if (!(_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated).GetValueOrDefault())
            return tags.Where(tag => !tag.IsPrivate).ToArray();

        return tags;
    }

    public async Task SaveTag(Tag tag, CancellationToken cancellationToken = default)
    {
        if (tag.Id == default)
            _ = _db.Tags.Add(tag);
        else
            _ = _db.Update(tag);

        _ = await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTag(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tag is null)
            throw new Exception($"Tag with ID {id} not found.");

        var photoTags = await _db.PhotoTags
            .Where(pt => pt.TagId == id)
            .ToArrayAsync(cancellationToken);

        _db.RemoveRange(photoTags);
        _ = _db.Remove(tag);
        _ = _db.SaveChangesAsync(cancellationToken);
    }
}
