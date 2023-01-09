using System.Net;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;

namespace TeacherPouch.Services;

public static class UrlHelperExtensions
{
    public static string PhotoUrl(this IUrlHelper _, Photo photo, PhotoSizes size) => $"/Photos/{photo.Id}/{CleansePhotoName(photo.Name)}_{size}.jpg";

    public static string SmallPhotoUrl(this IUrlHelper urlHelper, Photo photo) => urlHelper.PhotoUrl(photo, PhotoSizes.Small);

    public static string LargePhotoUrl(this IUrlHelper urlHelper, Photo photo) => urlHelper.PhotoUrl(photo, PhotoSizes.Large);

    public static string PhotoDownloadUrl(this IUrlHelper _, Photo photo, PhotoSizes size) =>
        $"/Photos/{photo.Id}/Download/{CleansePhotoName(photo.Name)}_{size}.jpg";

    public static string PhotoIndex(this IUrlHelper _) => "/PhotoIndex";

    public static string PhotoCreate(this IUrlHelper _) => "/Photos/Create";

    public static string PhotoDetails(this IUrlHelper urlHelper, Photo photo, Tag tag) => urlHelper.PhotoDetails(photo, tag.Name);

    public static string PhotoDetails(this IUrlHelper urlHelper, Photo photo, Tag? tag, Tag? tag2)
    {
        if (tag is not null && tag2 is not null)
            return urlHelper.PhotoDetails(photo, tag.Name, tag2.Name);
        else if (tag is not null)
            return urlHelper.PhotoDetails(photo, tag.Name);
        else
            return urlHelper.PhotoDetails(photo);
    }

    public static string PhotoDetails(this IUrlHelper _, Photo photo, string? tagName = null)
    {
        var url = string.Format("/Photos/{0}/{1}", photo.Id, CleansePhotoName(photo.Name));

        if (!string.IsNullOrWhiteSpace(tagName))
        {
            url = url + "?tag=" + tagName;
        }

        return url;
    }

    public static string PhotoDetails(this IUrlHelper urlHelper, Photo photo, string tagName, string tag2Name)
    {
        var url = urlHelper.PhotoDetails(photo, tagName);

        if (!string.IsNullOrWhiteSpace(tag2Name))
        {
            url = url + "&tag2=" + tag2Name;
        }

        return url;
    }

    public static string PhotoEdit(this IUrlHelper _, Photo photo) => $"/Photos/Edit/{photo.Id}";

    public static string PhotoDelete(this IUrlHelper _, Photo photo) => $"/Photos/Delete/{photo.Id}";


    public static string TagIndex(this IUrlHelper _) => "/TagIndex";

    public static string TagCreate(this IUrlHelper _) => "/Tags/CreateNew";

    public static string TagDetails(this IUrlHelper urlHelper, Tag tag) => TagDetails(urlHelper, tag.Name);

    public static string TagDetails(this IUrlHelper _, string tagName) => $"/Tags/{tagName}";

    public static string TagEdit(this IUrlHelper _, Tag tag) => $"/Tags/Edit/{tag.Id}";

    public static string TagDelete(this IUrlHelper _, Tag tag) => $"/Tags/Delete/{tag.Id}";


    public static string Search(this IUrlHelper _, string query) => $"/Search?q={query}";

    public static string CombinedSearch(this IUrlHelper _, string query) => $"/Search?q={query}&op=and";


    public static string Contact(this IUrlHelper _) => "/Contact";

    public static string About(this IUrlHelper _) => "/About";

    public static string Admin(this IUrlHelper _) => "/Admin";

    public static string AdminLogout(this IUrlHelper _, string? returnUrl = null)
    {
        var logoutUrl = "/Admin/Logout";

        if (!string.IsNullOrWhiteSpace(returnUrl))
            logoutUrl = logoutUrl + "?ReturnUrl=" + WebUtility.UrlEncode(returnUrl);

        return logoutUrl;
    }

    public static string Category(this IUrlHelper _, string name) => $"/Category/{name}";

    public static string License(this IUrlHelper _) => "/License";

    public static string Standards(this IUrlHelper _) => "/Standards";

    public static string PrivacyPolicy(this IUrlHelper _) => "/PrivacyPolicy";


    private static string? CleansePhotoName(string? photoName)
    {
        if (photoName is null)
            return photoName;
        else
            return photoName.Replace(' ', '-').Replace(".", string.Empty);
    }
}
