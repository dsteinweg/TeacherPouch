using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using TeacherPouch.Models;
using TeacherPouch.Utilities.Extensions;

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
                "<div href=\"{0}\" class=\"{1}\"><img src=\"{2}\" class=\"img-responsive center-block\" alt=\"{3}\"{4}></div>",
                urlHelper.PhotoDetails(photo),
                cssClasses,
                urlHelper.VersionedPhoto(photo, PhotoSizes.Large),
                tagNames,
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
                "<a href=\"{0}\" class=\"{1}\"><img src=\"{2}\" class=\"img-responsive\"{3}></a>",
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

            return new MvcHtmlString(
                String.Format(
                    "<a href=\"{0}?tag={1}\" class=\"{2}\"><img src=\"{3}\"{4}></a>",
                    urlHelper.PhotoDetails(photo),
                    tag.Name,
                    cssClasses,
                    urlHelper.VersionedPhoto(photo, PhotoSizes.Small),
                    title
                )
            );
        }

        public static MvcHtmlString PhotoThumb_AndedSearchResults(this HtmlHelper htmlHelper, Photo photo, IEnumerable<Tag> tags)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var cssClasses = "photo-thumb";
            string title = null;
            if (photo.IsPrivate)
            {
                cssClasses = cssClasses + " private";
                title = " title=\"This photo is private.\"";
            }

            string tagsQs = null;
            for (int i = 0; i < tags.Count(); i++)
            {
                if (i == 0)
                    tagsQs = "tag=" + tags.ElementAt(i).Name;
                else
                    tagsQs = tagsQs + "&tag" + (i + 1).ToString() + "=" + tags.ElementAt(i).Name;
            }

            return new MvcHtmlString(
                String.Format(
                    "<a href=\"{0}?{1}\" class=\"{2}\"><img src=\"{3}\"{4}></a>",
                    urlHelper.PhotoDetails(photo),
                    tagsQs,
                    cssClasses,
                    urlHelper.VersionedPhoto(photo, PhotoSizes.Small),
                    title
                )
            );
        }

        public static MvcHtmlString CombinedTags_SearchResult(this HtmlHelper htmlHelper, IEnumerable<Tag> tags)
        {
            if (!tags.SafeAny())
                return MvcHtmlString.Empty;

            var builder = new StringBuilder();

            var tagList = tags.ToList();
            for (int i = 0; i < tagList.Count - 1; i++)
            {
                builder.Append(TagButton(htmlHelper, tagList[i]));
                builder.Append(" + ");
            }

            builder.Append(TagButton(htmlHelper, tagList[tagList.Count - 1]));

            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString ConditionalAnalyticsScript(this HtmlHelper htmlHelper)
        {
            if (htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url.Host.Contains("teacherpouch.com", StringComparison.OrdinalIgnoreCase))
            {
                var scriptBody =
                    "<script>" +
                    "    (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){" +
                    "    (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o)," +
                    "    m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)" +
                    "    })(window,document,'script','//www.google-analytics.com/analytics.js','ga');" +
                    "    ga('create', 'UA-47264908-1', 'teacherpouch.com');" +
                    "    ga('send', 'pageview');" +
                    "</script>";

                return new MvcHtmlString(scriptBody);
            }
            else
            {
                return MvcHtmlString.Empty;
            }
        }
    }
}
