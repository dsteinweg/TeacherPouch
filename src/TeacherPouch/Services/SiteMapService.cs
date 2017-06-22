using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using TeacherPouch.Data;
using TeacherPouch.Models;

namespace TeacherPouch.Helpers
{
    public class SiteMapService
    {
        public SiteMapService(
            TeacherPouchDbContext dbContext,
            UrlHelper urlHelper)
        {
            _db = dbContext;
            _urlHelper = urlHelper;
        }

        private readonly TeacherPouchDbContext _db;
        private readonly UrlHelper _urlHelper;

        private static readonly XNamespace RootNamespace = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
        private static readonly XNamespace ImagesNamespace = XNamespace.Get("http://www.google.com/schemas/sitemap-image/1.1");

        public string GenerateSiteMapXml(HttpContext httpContext)
        {
            var siteMapFormat = GetSiteMapFormatString(httpContext);
            var protocolAndHost = httpContext.Request.Protocol + httpContext.Request.Host;
            var siteMap = String.Format(siteMapFormat, protocolAndHost);

            return siteMap;
        }

        private string GetSiteMapFormatString(HttpContext httpContext)
        {
            var root =
                new XElement(RootNamespace + "urlset",
                    new XAttribute(XNamespace.Xmlns + "image", ImagesNamespace));

            // General pages
            root.Add(CreateSiteMapNode("/"));
            root.Add(CreateSiteMapNode(_urlHelper.About()));
            root.Add(CreateSiteMapNode(_urlHelper.License()));
            root.Add(CreateSiteMapNode(_urlHelper.Contact()));
            root.Add(CreateSiteMapNode(_urlHelper.PrivacyPolicy()));

            // Categories
            var categories = Enum.GetNames(typeof(Category));
            foreach (var category in categories)
            {
                root.Add(CreateSiteMapNode(_urlHelper.Category(category)));
            }

            // Photos
            var photos = _db.Photos.Where(photo => !photo.IsPrivate);
            foreach (var photo in photos)
            {
                root.Add(CreatePhotoSiteMapNode(_urlHelper.PhotoDetails(photo), _urlHelper.LargePhotoUrl(photo), _urlHelper.License()));
            }

            // Tags
            var tags = _db.Tags.Where(tag => !tag.IsPrivate);
            foreach (var tag in tags)
            {
                root.Add(CreateSiteMapNode(_urlHelper.TagDetails(tag)));
            }

            using (var ms = new MemoryStream())
            {
                using (var writer = new StreamWriter(ms, Encoding.UTF8))
                {
                    root.Save(writer);
                }

                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private static XElement CreateSiteMapNode(string siteRelativeUrl)
        {
            return
                new XElement(RootNamespace + "url",
                    new XElement(RootNamespace + "loc", "{0}" + siteRelativeUrl));
        }

        private static XElement CreatePhotoSiteMapNode(string siteRelativeUrl, string photoImageUrl, string licenseUrl)
        {
            return
                new XElement(RootNamespace + "url",
                    new XElement(RootNamespace + "loc", "{0}" + siteRelativeUrl),
                    new XElement(ImagesNamespace + "image",
                        new XElement(ImagesNamespace + "loc", "{0}" + photoImageUrl),
                        new XElement(ImagesNamespace + "license", "{0}" + licenseUrl)));
        }
    }
}
