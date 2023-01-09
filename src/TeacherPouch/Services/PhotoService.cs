using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using TeacherPouch.Data;
using TeacherPouch.Models;
using TeacherPouch.Options;

namespace TeacherPouch.Services;

public class PhotoService
{
    public PhotoService(
        IOptions<PhotoPaths> photoPaths,
        TeacherPouchDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        PhotoPath = photoPaths.Value.PhotoPath;
        PendingPhotoPath = photoPaths.Value.PendingPhotoPath;
        _db = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly string PhotoPath;
    public readonly string PendingPhotoPath;
    private readonly TeacherPouchDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task<Photo[]> GetAllPhotos(CancellationToken cancellationToken = default)
    {
        if ((_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated).GetValueOrDefault())
            return await _db.Photos.ToArrayAsync(cancellationToken);

        return await _db.Photos
            .Where(photo => !photo.IsPrivate)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<PaginatedList<Photo>> GetPhotoIndexPage(int pageNumber, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var includePrivatePhotos = (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated).GetValueOrDefault();

        var query = _db.Photos
            .AsNoTracking()
            .Where(photo => includePrivatePhotos || !photo.IsPrivate);

        return await PaginatedList<Photo>.CreateAsync(query, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Photo?> FindPhoto(int id, CancellationToken cancellationToken = default)
    {
        var photo = await _db.Photos
            .Where(p => p.Id == id)
            .Include(p => p.PhotoTags)
            .ThenInclude(pt => pt.Tag)
            .Include(p => p.Questions)
            .FirstOrDefaultAsync(cancellationToken);

        if (photo is null)
            return null;

        if (!(_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated).GetValueOrDefault() && photo.IsPrivate)
            return null;

        return photo;
    }

    public string GetPhotoFilePath(Photo photo, PhotoSizes size)
    {
        return Path.Combine(
            PhotoPath,
            photo.UniqueId.ToString(),
            size.ToString().ToLower() + ".jpg");
    }

    public string? GetPhotoFileSize(Photo photo, PhotoSizes size)
    {
        var photoPath = GetPhotoFilePath(photo, size);
        var photoFileInfo = new FileInfo(photoPath);
        if (photoFileInfo.Exists)
        {
            var sizes = new[] { "B", "KB", "MB", "GB" };
            double len = photoFileInfo.Length;
            var order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
        else
        {
            return null;
        }
    }

    public async Task<byte[]?> GetPhotoBytes(Photo photo, PhotoSizes size, CancellationToken cancellationToken = default)
    {
        var photoPath = GetPhotoFilePath(photo, size);

        if (File.Exists(photoPath))
            return await File.ReadAllBytesAsync(photoPath, cancellationToken);
        else
            return null;
    }

    public void SavePhoto(Photo photo)
    {
        /*
        if (photo.ID == 0)
        {
            // Save the photo.
            var insertPhotoQuery = SQL.INSERT_INTO(string.Format("{0}({1})", PHOTO_TABLE_NAME, "Name, UniqueID, IsPrivate"))
                                      .VALUES(photo.Name, photo.UniqueID, photo.IsPrivate);

            using (var connection = ConnectionHelper.GetSQLiteConnection())
            {
                connection.Execute(insertPhotoQuery);
            }

            // Retrieve the photo to get its ID.
            var retrievedPhoto = FindPhoto(photo.UniqueID, true);

            if (retrievedPhoto is null)
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

        if (tagNames.Any())
        {
            var photoTags = new List<Tag>();

            // Create new tags.
            foreach (var tagName in tagNames)
            {
                var existingTag = FindTag(tagName, true);
                if (existingTag is null)
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
        */
    }

    public void DeletePhoto(Photo photo)
    {
        if (photo is null || photo.Id == 0)
            return;

        // TODO: implement delete

        /*
        PhotoTagAssociation.DeleteTagAssociationsForPhoto(photo);

        var deleteQuery = SQL.DELETE_FROM(PHOTO_TABLE_NAME)
                                .WHERE("ID = {0}", photo.ID);

        using (var connection = ConnectionHelper.GetSQLiteConnection())
        {
            connection.Execute(deleteQuery);
        }
        */
    }

    public void MovePhoto(string fileName, Photo photo)
    {
        AssertPhotoFoldersExist();

        var photoToMove = new FileInfo(Path.Combine(PendingPhotoPath, fileName));
        if (!photoToMove.Exists || photo is null || photo.UniqueId == Guid.Empty)
            return;

        var guid = photo.UniqueId.ToString().ToLower();

        var guidDirectoryPath = Path.Combine(PhotoPath, guid);
        var newFilePath = Path.Combine(guidDirectoryPath, fileName);

        _ = Directory.CreateDirectory(guidDirectoryPath);
        if (!File.Exists(newFilePath))
            photoToMove.MoveTo(newFilePath);
    }

    public async Task GeneratePhotoSizes(Photo photo, CancellationToken cancellationToken = default)
    {
        AssertPhotoFoldersExist();

        var singlePhotoFolderPath = Path.Combine(PhotoPath, photo.UniqueId.ToString());
        var files = new DirectoryInfo(singlePhotoFolderPath).GetFiles();

        if (files.Any())
        {
            var file = files.First();

            await ResizePhoto(file.FullName, PhotoSizes.Large, cancellationToken);
            await ResizePhoto(file.FullName, PhotoSizes.Small, cancellationToken);
        }
    }

    private void AssertPhotoFoldersExist()
    {
        if (!Directory.Exists(PendingPhotoPath))
            throw new DirectoryNotFoundException("Pending photos directory not found. App setting value: " + PendingPhotoPath);

        if (!Directory.Exists(PhotoPath))
            throw new DirectoryNotFoundException("Photos directory not found. App setting value: " + PhotoPath);
    }

    private static async Task ResizePhoto(string pathToOriginalPhoto, PhotoSizes size, CancellationToken cancellationToken = default)
    {
        var destinationPath = Path.Combine(Path.GetDirectoryName(pathToOriginalPhoto)!, size.ToString().ToLower() + ".jpg");

        using var originalImageStream = File.OpenRead(pathToOriginalPhoto);
        var (originalImage, format) = await Image.LoadWithFormatAsync(originalImageStream, cancellationToken);
        using (originalImage)
        {
            using var resizedImageStream = File.OpenWrite(destinationPath);

            var resizeOptions = new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Sampler = new WelchResampler()
            };

            switch (size)
            {
                case PhotoSizes.Small:
                    resizeOptions.Size = new Size(200, 200);
                    break;

                case PhotoSizes.Large:
                    resizeOptions.Size = new Size(800, 800);
                    break;

                default:
                    throw new Exception("Unrecognized photo size.");
            }

            originalImage.Mutate(context => context.Resize(resizeOptions));
            await originalImage.SaveAsync(resizedImageStream, format, cancellationToken);
        }
    }
}
