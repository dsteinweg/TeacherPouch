using System;
using System.Web.Mvc;

using TeacherPouch.Models;
using System.Web.Caching;
using System.IO;
using TeacherPouch.Utilities.Caching;

namespace TeacherPouch.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        private const string BASE_PHOTO_URL_FORMAT = "/photos/{0}/{1}";


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
                    "Versioned Script/CSS File - " + contentPath,
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

        public static string SmallPhotoUrl(this UrlHelper urlHelper, Photo photo)
        {
            return String.Format(BASE_PHOTO_URL_FORMAT, photo.UniqueID.ToString(), PhotoSizes.Small.ToString() + ".jpg").ToLower();
        }

        public static string LargePhotoUrl(this UrlHelper urlHelper, Photo photo)
        {
            return String.Format(BASE_PHOTO_URL_FORMAT, photo.UniqueID.ToString(), PhotoSizes.Large.ToString() + ".jpg").ToLower();
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

        public static string PhotoDetails(this UrlHelper urlHelper, Photo photo, string tagName = null)
        {
            var url = String.Format("/Photos/Details/{0}", photo.ID);

            if (!String.IsNullOrWhiteSpace(tagName))
            {
                url = url + "?tag=" + tagName;
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

        public static string Copyright(this UrlHelper urlHelper)
        {
            return "/Copyright";
        }
    }
}