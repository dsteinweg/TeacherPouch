using TeacherPouch.Models;

namespace TeacherPouch.ViewModels;

public class PhotoDetailsViewModel(
    Photo photo,
    string photoUrl,
    string? smallFileSize,
    string smallDownloadUrl,
    string? largeFileSize,
    string largeDownloadUrl,
    Tag? searchResultTag,
    Tag? searchResultTag2,
    Photo? previousPhoto,
    Photo? nextPhoto,
    bool userIsAdmin)
{
    public Photo Photo { get; } = photo;
    public string PhotoUrl { get; } = photoUrl;
    public IEnumerable<Tag> Tags { get; } = photo.PhotoTags.Select(pt => pt.Tag);
    public IEnumerable<Question> Questions { get; } = photo.Questions;
    public string? SmallFileSize { get; } = smallFileSize;
    public string SmallDownloadUrl { get; } = smallDownloadUrl;
    public string? LargeFileSize { get; } = largeFileSize;
    public string LargeDownloadUrl { get; } = largeDownloadUrl;
    public Tag? SearchResultTag { get; } = searchResultTag;
    public Tag? SearchResultTag2 { get; } = searchResultTag2;
    public Photo? PreviousPhoto { get; } = previousPhoto;
    public Photo? NextPhoto { get; } = nextPhoto;
    public bool ShowAdminLinks { get; } = userIsAdmin;
}
