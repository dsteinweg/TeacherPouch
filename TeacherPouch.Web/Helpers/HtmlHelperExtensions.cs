using System;
using System.Linq;
using System.Collections.Generic;
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

            var buttonHtml = String.Format(
                "<a href=\"{0}\" class=\"{1}\"{2}><span>{3}</span></a>",
                urlHelper.TagDetails(tag),
                cssClasses,
                title,
                tag.Name
            );

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

            var buttonHtml = String.Format(
                "<span class=\"{0}\"{1}>{2}</span>",
                cssClasses,
                title,
                tag.Name
            );

            return new MvcHtmlString(buttonHtml);
        }

        public static MvcHtmlString LargePhoto(this HtmlHelper htmlHelper, Photo photo, IEnumerable<Tag> photoTags)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var tagNames = String.Join(" ", photoTags.Select(tag => tag.Name)).Replace("\"", String.Empty);

            var cssClasses = "photo-thumb";
            string title = null;
            if (photo.IsPrivate)
            {
                cssClasses = cssClasses + " private";
                title = " title=\"This photo is private.\"";
            }

            var thumbHtml = String.Format(
                "<div href=\"{0}\" class=\"{1}\"><img alt=\"{2}\" src=\"{3}\"{4}></div>",
                urlHelper.PhotoDetails(photo),
                cssClasses,
                tagNames,
                urlHelper.VersionedPhoto(photo, PhotoSizes.Large),
                title
            );

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

            var thumbHtml = String.Format(
                "<a href=\"{0}\" class=\"{1}\"><img src=\"{2}\"{3}></a>",
                urlHelper.PhotoDetails(photo),
                cssClasses,
                urlHelper.VersionedPhoto(photo, PhotoSizes.Small),
                title
            );

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

            var thumbHtml = String.Format(
                "<a href=\"{0}?tag={1}\" class=\"{2}\"><img src=\"{3}\"{4}></a>",
                urlHelper.PhotoDetails(photo),
                tag.Name,
                cssClasses,
                urlHelper.VersionedPhoto(photo, PhotoSizes.Small),
                title
            );

            return new MvcHtmlString(thumbHtml);
        }
    }
}
