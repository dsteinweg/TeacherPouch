using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Routing;
using TeacherPouch.Models;

namespace TeacherPouch.Helpers
{
    public static class UrlHelperExtensions
    {
        private const string BASE_PHOTO_URL_FORMAT = "/Photos/{0}/{1}_{2}.jpg";
        private const string BASE_PHOTO_DOWNLOAD_URL_FORMAT = "/Photos/{0}/Download/{1}_{2}.jpg";

        public static string PhotoUrl(this UrlHelper urlHelper, Photo photo, PhotoSizes size)
        {
            return String.Format(BASE_PHOTO_URL_FORMAT, photo.Id.ToString(), CleansePhotoName(photo.Name), size.ToString());
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
            return String.Format(BASE_PHOTO_DOWNLOAD_URL_FORMAT, photo.Id.ToString(), CleansePhotoName(photo.Name), size.ToString());
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
            var url = String.Format("/Photos/{0}/{1}", photo.Id, CleansePhotoName(photo.Name));

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
            return String.Format("/Photos/Edit/{0}", photo.Id);
        }

        public static string PhotoDelete(this UrlHelper urlHelper, Photo photo)
        {
            return String.Format("/Photos/Delete/{0}", photo.Id);
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
            return String.Format("/Tags/Edit/{0}", tag.Id);
        }

        public static string TagDelete(this UrlHelper urlHelper, Tag tag)
        {
            return String.Format("/Tags/Delete/{0}", tag.Id);
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
                logoutUrl = logoutUrl + "?ReturnUrl=" + WebUtility.UrlEncode(returnUrl);

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

        public static string Standards(this UrlHelper urlHelper)
        {
            return "/Standards";
        }

        public static string PrivacyPolicy(this UrlHelper urlHelper)
        {
            return "/PrivacyPolicy";
        }


        private static string CleansePhotoName(string photoName)
        {
            if (photoName == null)
                return photoName;
            else
                return photoName.Replace(' ', '-').Replace(".", String.Empty);
        }
    }
}