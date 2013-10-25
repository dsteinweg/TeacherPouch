using System;
using System.Configuration;
using System.IO;
using System.Linq;
using ImageResizer;

using TeacherPouch.Models;

namespace TeacherPouch.Utilities
{
    public static class PhotoHelper
    {
        public static readonly string PhotoPath = null;
        public static readonly string PendingPhotoPath = null;


        static PhotoHelper()
        {
            PhotoPath = ConfigurationManager.AppSettings["PhotoPath"];

            if (String.IsNullOrWhiteSpace(PhotoPath))
                throw new ConfigurationErrorsException("\"PhotoPath\" app setting missing.");


            PendingPhotoPath = ConfigurationManager.AppSettings["PendingPhotoPath"];

            if (String.IsNullOrWhiteSpace(PendingPhotoPath))
                throw new ConfigurationErrorsException("\"PendingPhotoPath\" app setting missing.");
        }


        private static void AssertPhotoFoldersExist()
        {
            if (!Directory.Exists(PendingPhotoPath))
                throw new DirectoryNotFoundException("Pending photos directory not found.  App setting value: " + PendingPhotoPath);

            if (!Directory.Exists(PhotoPath))
                throw new DirectoryNotFoundException("Photos directory not found.  App setting value: " + PhotoPath);
        }

        public static void MovePhoto(string fileName, Photo photo)
        {
            AssertPhotoFoldersExist();

            var photoToMove = new FileInfo(Path.Combine(PendingPhotoPath, fileName));
            if (photoToMove.Exists)
            {
                if (photo != null && photo.UniqueID != Guid.Empty)
                {
                    string guid = photo.UniqueID.ToString().ToLower();

                    string guidDirectoryPath = Path.Combine(PhotoPath, guid);
                    string newFilePath = Path.Combine(guidDirectoryPath, fileName);

                    Directory.CreateDirectory(guidDirectoryPath);
                    if (!File.Exists(newFilePath))
                    {
                        photoToMove.MoveTo(newFilePath);
                    }
                }
            }
        }


        public static void GeneratePhotoSizes(Photo photo)
        {
            AssertPhotoFoldersExist();

            string singlePhotoFolderPath = Path.Combine(PhotoPath, photo.UniqueID.ToString());
            var files = new DirectoryInfo(singlePhotoFolderPath).GetFiles();

            if (files.Any())
            {
                var file = files.First();

                ResizeImage(file.FullName, PhotoSizes.Small);
                ResizeImage(file.FullName, PhotoSizes.Large);
            }
        }

        private static void ResizeImage(string pathToOriginalPhoto, PhotoSizes size)
        {
            var destinationPath = Path.Combine(Path.GetDirectoryName(pathToOriginalPhoto), size.ToString().ToLower() + ".jpg");

            var resizeSettings = new ResizeSettings();
            switch (size)
            {
                case PhotoSizes.Small:
                    resizeSettings.MaxWidth = 200;
                    resizeSettings.MaxHeight = 200;
                    break;

                case PhotoSizes.Medium:
                    resizeSettings.MaxWidth = 500;
                    resizeSettings.MaxHeight = 500;
                    break;

                case PhotoSizes.Large:
                    resizeSettings.MaxWidth = 800;
                    resizeSettings.MaxHeight = 800;
                    break;
            }

            ImageBuilder.Current.Build(
                source: pathToOriginalPhoto,
                dest: destinationPath,
                settings: resizeSettings
            );
        }
    }
}
