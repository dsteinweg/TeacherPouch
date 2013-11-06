using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

using TeacherPouch.Repositories;
using TeacherPouch.Utilities.Caching;
using TeacherPouch.Web.Controllers;

namespace TeacherPouch.Web.Helpers
{
    public static class SiteMapHelper
    {
        private static readonly XNamespace RootNamespace = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
        private static readonly XNamespace ImagesNamespace = XNamespace.Get("http://www.google.com/schemas/sitemap-image/1.1");

        public static string GetSiteMap(IRepository repository, HttpContextBase httpContext)
        {
            var siteMapFormat = GetSiteMapFormatString(repository, httpContext);
            var protocolAndHost = httpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            var siteMap = String.Format(siteMapFormat, protocolAndHost);

            return siteMap;
        }

        private static string GetSiteMapFormatString(IRepository repository, HttpContextBase httpContext)
        {
            var urlHelper = new UrlHelper(httpContext.Request.RequestContext);

            var root =
                new XElement(RootNamespace + "urlset",
                    new XAttribute(XNamespace.Xmlns + "image", ImagesNamespace)
            );

            // General pages
            root.Add(CreateSiteMapNode("/"));
            root.Add(CreateSiteMapNode(urlHelper.About()));
            root.Add(CreateSiteMapNode(urlHelper.Contact()));
            root.Add(CreateSiteMapNode(urlHelper.License()));

            // Categories
            var categories = Enum.GetNames(typeof(CategoryController.Category));
            foreach (var category in categories)
            {
                root.Add(CreateSiteMapNode(urlHelper.Category(category)));
            }
            
            // Photos
            var photos = repository.GetAllPhotos(false);
            foreach (var photo in photos)
            {
                root.Add(CreatePhotoSiteMapNode(urlHelper.PhotoDetails(photo), urlHelper.LargePhotoUrl(photo), urlHelper.License()));
            }


            // Tags
            var tags = repository.GetAllTags(false);
            foreach (var tag in tags)
            {
                root.Add(CreateSiteMapNode(urlHelper.TagDetails(tag)));
            }


            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    root.Save(writer);
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static XElement CreateSiteMapNode(string siteRelativeUrl)
        {
            return new XElement(RootNamespace + "url",
                    new XElement(RootNamespace + "loc", "{0}" + siteRelativeUrl));
        }

        private static XElement CreatePhotoSiteMapNode(string siteRelativeUrl, string photoImageUrl, string licenseUrl)
        {
            return
                new XElement(RootNamespace + "url",
                    new XElement(RootNamespace + "loc", "{0}" + siteRelativeUrl),
                    new XElement(ImagesNamespace + "image",
                        new XElement(ImagesNamespace + "loc", "{0}" + photoImageUrl),
                        new XElement(ImagesNamespace + "license", "{0}" + licenseUrl)
                    )
                );
        }
    }
}