/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TeacherPouch.Models;*/

namespace TeacherPouch.Helpers
{
    public static class HtmlHelperExtensions
    {
        /*
        public static HtmlString PhotoThumb(this HtmlHelper htmlHelper, Photo photo)
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

            return new HtmlString(thumbHtml);
        }

        public static HtmlString PhotoThumb_SearchResult(this HtmlHelper htmlHelper, Photo photo, Tag tag)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var cssClasses = "photo-thumb";
            string title = null;
            if (photo.IsPrivate)
            {
                cssClasses = cssClasses + " private";
                title = " title=\"This photo is private.\"";
            }

            return new HtmlString(
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

        public static HtmlString PhotoThumb_AndedSearchResults(this HtmlHelper htmlHelper, Photo photo, IEnumerable<Tag> tags)
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

            return new HtmlString(
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

        public static HtmlString CombinedTags_SearchResult(this HtmlHelper htmlHelper, IEnumerable<Tag> tags)
        {
            if (!tags.Any())
                return HtmlString.Empty;

            var builder = new StringBuilder();

            var tagList = tags.ToList();
            for (int i = 0; i < tagList.Count - 1; i++)
            {
                builder.Append(TagButton(htmlHelper, tagList[i]));
                builder.Append(" + ");
            }

            builder.Append(TagButton(htmlHelper, tagList[tagList.Count - 1]));

            return new HtmlString(builder.ToString());
        }
        */
    }
}
