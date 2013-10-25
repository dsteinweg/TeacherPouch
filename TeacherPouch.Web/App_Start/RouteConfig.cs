using System.Web.Mvc;
using System.Web.Routing;

namespace TeacherPouch.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*robots}", new { robotstxt = "(.*/)?robots.txt(/.*)?" });
            routes.IgnoreRoute("{*favicon}", new { favicon = "(.*/)?favicon.ico(/.*)?" });

            // Pages routes
            routes.MapRoute(
                name: "Pages - Home",
                url: "",
                defaults: MVC.Pages.Home()
            );
            routes.MapRoute(
                name: "Pages - About",
                url: "About",
                defaults: MVC.Pages.About()
            );
            routes.MapRoute(
                name: "Pages - Contact",
                url: "Contact",
                defaults: MVC.Pages.Contact()
            );
            routes.MapRoute(
                name: "Pages - Contact Thanks",
                url: "Contact/Thanks",
                defaults: MVC.Pages.ContactThanks()
            );
            routes.MapRoute(
                name: "Pages - Copyright",
                url: "Copyright",
                defaults: MVC.Pages.Copyright()
            );


            // Search routes
            routes.MapRoute(
                name: "Search",
                url: "Search",
                defaults: MVC.Search.Search()
            );


            // Category routes
            routes.MapRoute(
                name: "Category Details",
                url: "Category/{name}",
                defaults: MVC.Category.CategoryDetails()
            );


            // Tag routes
            routes.MapRoute(
                name: "Create Tag",
                url: "Tags/CreateNew",
                defaults: MVC.Tags.TagCreate()
            );
            routes.MapRoute(
                name: "Tag Edit",
                url: "Tags/Edit/{id}",
                defaults: MVC.Tags.TagEdit()
            );
            routes.MapRoute(
                name: "Tag Delete",
                url: "Tags/Delete/{id}",
                defaults: MVC.Tags.TagDelete()
            );
            routes.MapRoute(
                name: "Tag Details",
                url: "Tags/{tagName}",
                defaults: MVC.Tags.TagDetails()
            );
            routes.MapRoute(
                name: "Tag Index",
                url: "TagIndex",
                defaults: MVC.Tags.TagIndex()
            );


            // Photo routes
            routes.MapRoute(
                name: "Photo Details",
                url: "Photos/Details/{id}",
                defaults: MVC.Photos.PhotoDetails(),
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "Photo Index",
                url: "PhotoIndex",
                defaults: MVC.Photos.PhotoIndex()
            );
            routes.MapRoute(
                name: "Create Photo",
                url: "Photos/Create",
                defaults: MVC.Photos.PhotoCreate()
            );
            routes.MapRoute(
                name: "Edit Photo",
                url: "Photos/Edit/{id}",
                defaults: MVC.Photos.PhotoEdit(),
                constraints: new { id = @"\d+" }
            );
            routes.MapRoute(
                name: "Delete Photo",
                url: "Photos/Delete/{id}",
                defaults: MVC.Photos.PhotoDelete(),
                constraints: new { id = @"\d+" }
            );


            // Admin routes
            routes.MapRoute(
                name: "Admin",
                url: "Admin",
                defaults: MVC.Admin.Index()
            );
            routes.MapRoute(
                name: "Admin Login",
                url: "Admin/Login",
                defaults: MVC.Admin.Login()
            );
            routes.MapRoute(
                name: "Admin Logout",
                url: "Admin/Logout",
                defaults: MVC.Admin.Logout()
            );


            // Final catchall for handling unrecognized routes, returning a HTTP 404
            routes.MapRoute(
                name: "NotFound",
                url: "{*url}",
                defaults: MVC.Error.Http404()
            );
        }
    }
}
