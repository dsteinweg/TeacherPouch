using System;
using System.IO;
using System.Web.Caching;
using System.Web.Mvc;

using TeacherPouch.Models;
using TeacherPouch.Utilities;
using TeacherPouch.Utilities.Caching;

namespace TeacherPouch.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        private const string BASE_PHOTO_URL_FORMAT = "/Photos/{0}/{1}_{2}.jpg";
        private const string BASE_PHOTO_DOWNLOAD_URL_FORMAT = "/Photos/{0}/Download/{1}_{2}.jpg";


        /// <summary>
        /// Produces a versioned file URL, based off the file's last modified timestamp.
        /// </summary>
        public static string VersionedContent(this UrlHelper urlHelper, string contentPath)
        {
            var path = urlHelper.Content(contentPath);
            var fullFilePath = urlHelper.RequestContext.HttpContext.Server.MapPath(path);

            if (File.Exists(fullFilePath))
            {
                return CacheHelper.RetrieveFromCache(
                    "Versioned Content File - " + contentPath,
                    delegate()
                    {
                        var fileInfo = new FileInfo(fullFilePath);
                        return path + String.Format("?v={0}", fileInfo.LastWriteTime.Ticks);
                    },
                    new CacheDependency(fullFilePath),
                    TimeSpan.Zero
                );
            }
            else
            {
                return contentPath;
            }
        }

        /// <summary>
        /// Produces a versioned photo URL, based off the photo's last modified timestamp.
        /// </summary>
        public static string VersionedPhoto(this UrlHelper urlHelper, Photo photo, PhotoSizes size)
        {
            var path = PhotoHelper.GetPhotoFilePath(photo, size);

            if (File.Exists(path))
            {
                return CacheHelper.RetrieveFromCache(
                    "Versioned Photo - " + path,
                    delegate()
                    {
                        var fileInfo = new FileInfo(path);
                        return urlHelper.PhotoUrl(photo, size) + String.Format("?v={0}", fileInfo.LastWriteTime.Ticks);
                    },
                    new CacheDependency(path),
                    TimeSpan.Zero
                );
            }
            else
            {
                return path;
            }
        }

        public static string PhotoUrl(this UrlHelper urlHelper, Photo photo, PhotoSizes size)
        {
            return String.Format(BASE_PHOTO_URL_FORMAT, photo.ID.ToString(), photo.Name.Replace(' ', '-'), size.ToString());
        }

        public static string SmallPhotoUrl(this UrlHelper urlHelper, Photo photo)
        {
            return urlHelper.PhotoUrl(photo, PhotoSizes.Small);
        }

        public static string LargePhotoUrl(this UrlHelper urlHelper, Photo photo)
        {
            return urlHelper.PhotoUrl(photo, PhotoSizes.Large);
        }

        public static string PhotoDownloadUrl(this UrlHelper urlhelper, Photo photo, PhotoSizes size)
        {
            return String.Format(BASE_PHOTO_DOWNLOAD_URL_FORMAT, photo.ID.ToString(), photo.Name.Replace(' ', '-'), size.ToString());
        }

        public static string PhotoIndex(this UrlHelper urlHelper)
        {
            return "/PhotoIndex";
        }

        public static string PhotoCreate(this UrlHelper urlHelper)
        {
            return "/Photos/Create";
        }

        public static string PhotoDetails(this UrlHelper urlHelper, Photo photo, Tag tag)
        {
            return urlHelper.PhotoDetails(photo, tag.Name);
        }

        public static string PhotoDetails(this UrlHelper urlHelper, Photo photo, Tag tag, Tag tag2)
        {
            if (tag != null && tag2 != null)
                return urlHelper.PhotoDetails(photo, tag.Name, tag2.Name);
            else if (tag != null)
                return urlHelper.PhotoDetails(photo, tag.Name);
            else
                return urlHelper.PhotoDetails(photo);
        }

        public static string PhotoDetails(this UrlHelper urlHelper, Photo photo, string tagName = null)
        {
            var url = String.Format("/Photos/{0}/{1}", photo.ID, photo.Name.Replace(' ', '-'));

            if (!String.IsNullOrWhiteSpace(tagName))
            {
                url = url + "?tag=" + tagName;
            }

            return url;
        }

        public static string PhotoDetails(this UrlHelper urlHelper, Photo photo, string tagName, string tag2Name)
        {
            var url = urlHelper.PhotoDetails(photo, tagName);

            if (!String.IsNullOrWhiteSpace(tag2Name))
            {
                url = url + "&tag2=" + tag2Name;
            }

            return url;
        }

        public static string PhotoEdit(this UrlHelper urlHelper, Photo photo)
        {
            return String.Format("/Photos/Edit/{0}", photo.ID);
        }

        public static string PhotoDelete(this UrlHelper urlHelper, Photo photo)
        {
            return String.Format("/Photos/Delete/{0}", photo.ID);
        }


        public static string TagIndex(this UrlHelper urlHelper)
        {
            return "/TagIndex";
        }

        public static string TagCreate(this UrlHelper urlHelper)
        {
            return "/Tags/CreateNew";
        }

        public static string TagDetails(this UrlHelper urlHelper, Tag tag)
        {
            return TagDetails(urlHelper, tag.Name);
        }

        public static string TagDetails(this UrlHelper urlHelper, string tagName)
        {
            return String.Format("/Tags/{0}", tagName);
        }

        public static string TagEdit(this UrlHelper urlHelper, Tag tag)
        {
            return String.Format("/Tags/Edit/{0}", tag.ID);
        }

        public static string TagDelete(this UrlHelper urlHelper, Tag tag)
        {
            return String.Format("/Tags/Delete/{0}", tag.ID);
        }


        public static string Search(this UrlHelper urlHelper, string query)
        {
            return String.Format("/Search?q={0}", query);
        }

        public static string CombinedSearch(this UrlHelper urlHelper, string query)
        {
            return String.Format("/Search?q={0}&op=and", query);
        }


        public static string Contact(this UrlHelper urlHelper)
        {
            return "/Contact";
        }

        public static string About(this UrlHelper urlHelper)
        {
            return "/About";
        }

        public static string Admin(this UrlHelper urlHelper)
        {
            return "/Admin";
        }

        public static string AdminLogout(this UrlHelper urlHelper, string returnUrl = null)
        {
            var logoutUrl = "/Admin/Logout";

            if (!String.IsNullOrWhiteSpace(returnUrl))
                logoutUrl = logoutUrl + "?ReturnUrl=" + urlHelper.Encode(returnUrl);

            return logoutUrl;
        }

        public static string Category(this UrlHelper urlHelper, string name)
        {
            return String.Format("/Category/{0}", name);
        }

        public static string License(this UrlHelper urlHelper)
        {
            return "/License";
        }

        public static string PrivacyPolicy(this UrlHelper urlHelper)
        {
            return "/PrivacyPolicy";
        }
    }
}