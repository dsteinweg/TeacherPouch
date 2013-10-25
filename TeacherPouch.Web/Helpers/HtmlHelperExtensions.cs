using System;
using System.Web.Mvc;

using TeacherPouch.Models;

namespace TeacherPouch.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString TagButton(this HtmlHelper htmlHelper, Tag tag)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var cssClasses = "tag";
            string title = null;
            if (tag.IsPrivate)
            {
                cssClasses = cssClasses + " private";
                title = " title=\"This tag is private.\"";
            }

            var buttonHtmlFormat = "<a href=\"{0}\" class=\"{1}\"{2}><span>{3}</span></a>";
            var buttonHtml = String.Format(buttonHtmlFormat, urlHelper.TagDetails(tag), cssClasses, title, tag.Name);

            return new MvcHtmlString(buttonHtml);
        }

        public static MvcHtmlString TagButton_NoLink(this HtmlHelper htmlHelper, Tag tag)
        {
            var cssClasses = "tag";
            string title = null;
            if (tag.IsPrivate)
            {
                cssClasses = cssClasses + " private";
                title = " title=\"This tag is private.\"";
            }

            var buttonHtmlFormat = "<span class=\"{0}\"{1}>{2}</span>";
            var buttonHtml = String.Format(buttonHtmlFormat, cssClasses, title, tag.Name);

            return new MvcHtmlString(buttonHtml);
        }

        public static MvcHtmlString LargePhoto(this HtmlHelper htmlHelper, Photo photo)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var cssClasses = "photo-thumb";
            string title = null;
            if (photo.IsPrivate)
            {
                cssClasses = cssClasses + " private";
                title = " title=\"This photo is private.\"";
            }

            var thumbHtmlFormat = "<div href=\"{0}\" class=\"{1}\"><img src=\"{2}\"{3}></div>";
            var thumbHtml = String.Format(thumbHtmlFormat, urlHelper.PhotoDetails(photo), cssClasses, urlHelper.VersionedContent(urlHelper.LargePhotoUrl(photo)), title);

            return new MvcHtmlString(thumbHtml);
        }

        public static MvcHtmlString PhotoThumb(this HtmlHelper htmlHelper, Photo photo)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var cssClasses = "photo-thumb";
            string title = null;
            if (photo.IsPrivate)
            {
                cssClasses = cssClasses + " private";
                title = " title=\"This photo is private.\"";
            }

            var thumbHtmlFormat = "<a href=\"{0}\" class=\"{1}\"><img src=\"{2}\"{3}></a>";
            var thumbHtml = String.Format(thumbHtmlFormat, urlHelper.PhotoDetails(photo), cssClasses, urlHelper.VersionedContent(urlHelper.SmallPhotoUrl(photo)), title);

            return new MvcHtmlString(thumbHtml);
        }

        public static MvcHtmlString PhotoThumb_SearchResult(this HtmlHelper htmlHelper, Photo photo, Tag tag)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var cssClasses = "photo-thumb";
            string title = null;
            if (photo.IsPrivate)
            {
                cssClasses = cssClasses + " private";
                title = " title=\"This photo is private.\"";
            }

            var photoUrl = urlHelper.SmallPhotoUrl(photo);

            var thumbHtml = String.Format(
                "<a href=\"{0}?tag={1}\" class=\"{2}\"><img src=\"{3}\"{4}></a>",
                urlHelper.PhotoDetails(photo),
                tag.Name,
                cssClasses,
                urlHelper.VersionedContent(photoUrl),
                title
            );

            return new MvcHtmlString(thumbHtml);
        }
    }
}
