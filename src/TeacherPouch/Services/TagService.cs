using Microsoft.EntityFrameworkCore;
using TeacherPouch.Data;
using TeacherPouch.Models;

namespace TeacherPouch.Services;

public class TagService(IHttpContextAccessor _httpContextAccessor, TeacherPouchDbContext _dbContext)
{
    public async Task<Tag[]> GetAllTags(CancellationToken cancellationToken = default)
    {
        if ((_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated).GetValueOrDefault())
            return await _dbContext.Tags.ToArrayAsync(cancellationToken);

        return await _dbContext.Tags
            .Where(tag => !tag.IsPrivate)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Tag?> FindTag(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _dbContext.Tags
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
        var tags = await _dbContext.Tags
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
        var tags = await _dbContext.Tags.Where(t => t.Name.Contains(name)).ToArrayAsync(cancellationToken);

        if (!(_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated).GetValueOrDefault())
            return tags.Where(tag => !tag.IsPrivate).ToArray();

        return tags;
    }

    public async Task SaveTag(Tag tag, CancellationToken cancellationToken = default)
    {
        if (tag.Id == default)
            _ = _dbContext.Tags.Add(tag);
        else
            _ = _dbContext.Update(tag);

        _ = await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTag(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _dbContext.Tags.FirstAsync(t => t.Id == id, cancellationToken);
        var photoTags = await _dbContext.PhotoTags
            .Where(pt => pt.TagId == id)
            .ToArrayAsync(cancellationToken);

        _dbContext.RemoveRange(photoTags);
        _ = _dbContext.Remove(tag);
        _ = _dbContext.SaveChangesAsync(cancellationToken);
    }
}
