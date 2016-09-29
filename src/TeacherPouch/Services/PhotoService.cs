using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageProcessorCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TeacherPouch.Data;
using TeacherPouch.Models;
using TeacherPouch.Options;

namespace TeacherPouch.Services
{
    public class PhotoService
    {
        public PhotoService(
            IOptions<PhotoPaths> photoPaths,
            TeacherPouchDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            PhotoPath = photoPaths.Value.PhotoPath;
            PendingPhotoPath = photoPaths.Value.PendingPhotoPath;
            _db = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public readonly string PhotoPath = null;
        public readonly string PendingPhotoPath = null;
        private readonly TeacherPouchDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public IEnumerable<Photo> GetAllPhotos()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return _db.Photos.ToList();

            return _db.Photos.Where(photo => !photo.IsPrivate).ToList();
        }

        public Photo FindPhoto(int id)
        {
            var photo = _db.Photos.FirstOrDefault(p => p.Id == id);
            if (photo == null)
                return null;

            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && photo.IsPrivate)
                return null;

            return photo;
        }

        public string GetPhotoFilePath(Photo photo, PhotoSizes size)
        {
            return Path.Combine(PhotoPath, photo.UniqueId.ToString(), size.ToString() + ".jpg");
        }

        public string GetPhotoFileSize(Photo photo, PhotoSizes size)
        {
            var photoPath = GetPhotoFilePath(photo, size);
            var photoFileInfo = new FileInfo(photoPath);
            if (photoFileInfo.Exists)
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = photoFileInfo.Length;
                int order = 0;
                while (len >= 1024 && order + 1 < sizes.Length)
                {
                    order++;
                    len = len / 1024;
                }

                return String.Format("{0:0.##} {1}", len, sizes[order]);
            }
            else
            {
                return null;
            }
        }

        public byte[] GetPhotoBytes(Photo photo, PhotoSizes size)
        {
            var photoPath = GetPhotoFilePath(photo, size);

            if (File.Exists(photoPath))
                return File.ReadAllBytes(photoPath);
            else
                return null;
        }

        public void SavePhoto(Photo photo)
        {
            /*
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

            if (tagNames.Any())
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
            */
        }

        public void DeletePhoto(Photo photo)
        {
            if (photo == null || photo.Id == 0)
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
            if (!photoToMove.Exists || photo == null || photo.UniqueId == Guid.Empty)
                return;

            var guid = photo.UniqueId.ToString().ToLower();

            var guidDirectoryPath = Path.Combine(PhotoPath, guid);
            var newFilePath = Path.Combine(guidDirectoryPath, fileName);

            Directory.CreateDirectory(guidDirectoryPath);
            if (!File.Exists(newFilePath))
                photoToMove.MoveTo(newFilePath);
        }

        public void GeneratePhotoSizes(Photo photo)
        {
            AssertPhotoFoldersExist();

            string singlePhotoFolderPath = Path.Combine(PhotoPath, photo.UniqueId.ToString());
            var files = new DirectoryInfo(singlePhotoFolderPath).GetFiles();

            if (files.Any())
            {
                var file = files.First();

                ResizePhoto(file.FullName, PhotoSizes.Small);
                ResizePhoto(file.FullName, PhotoSizes.Large);
            }
        }

        private void AssertPhotoFoldersExist()
        {
            if (!Directory.Exists(PendingPhotoPath))
                throw new DirectoryNotFoundException("Pending photos directory not found. App setting value: " + PendingPhotoPath);

            if (!Directory.Exists(PhotoPath))
                throw new DirectoryNotFoundException("Photos directory not found. App setting value: " + PhotoPath);
        }

        private void ResizePhoto(string pathToOriginalPhoto, PhotoSizes size)
        {
            var destinationPath = Path.Combine(Path.GetDirectoryName(pathToOriginalPhoto), size.ToString().ToLower() + ".jpg");

            using (var originalStream = File.OpenRead(pathToOriginalPhoto))
            {
                var originalImage = new Image(originalStream);

                using (var resizedImageStream = File.OpenWrite(destinationPath))
                {
                    var resizeOptions = new ResizeOptions()
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

                    originalImage
                        .Resize(resizeOptions)
                        .Save(resizedImageStream);
                }
            }
        }
    }
}
