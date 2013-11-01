﻿using System;
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


        public static string GetPhotoFilePath(Photo photo, PhotoSizes size)
        {
            return Path.Combine(PhotoPath, photo.UniqueID.ToString(), size.ToString() + ".jpg");
        }

        public static string GetPhotoFileSize(Photo photo, PhotoSizes size)
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

        public static byte[] GetPhotoBytes(Photo photo, PhotoSizes size)
        {
            var photoPath = GetPhotoFilePath(photo, size);

            if (File.Exists(photoPath))
                return File.ReadAllBytes(photoPath);
            else
                return null;
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


        private static void AssertPhotoFoldersExist()
        {
            if (!Directory.Exists(PendingPhotoPath))
                throw new DirectoryNotFoundException("Pending photos directory not found.  App setting value: " + PendingPhotoPath);

            if (!Directory.Exists(PhotoPath))
                throw new DirectoryNotFoundException("Photos directory not found.  App setting value: " + PhotoPath);
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
