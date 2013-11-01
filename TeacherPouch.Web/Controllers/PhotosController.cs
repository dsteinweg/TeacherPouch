using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using TeacherPouch.Models;
using TeacherPouch.Models.Exceptions;
using TeacherPouch.Providers;
using TeacherPouch.Repositories;
using TeacherPouch.Utilities;
using TeacherPouch.Utilities.Caching;
using TeacherPouch.Web.ViewModels;

namespace TeacherPouch.Web.Controllers
{
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public partial class PhotosController : RepositoryControllerBase
    {
        public PhotosController(IRepository repository)
        {
            base.Repository = repository;
        }


        // GET: /PhotoIndex/
        [AllowAnonymous]
        public virtual ViewResult PhotoIndex()
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var photos = base.Repository.GetAllPhotos(allowPrivate).OrderBy(photo => photo.ID);

            return View(Views.PhotoIndex, photos);
        }

        // GET: /Photos/5/Flowers?tag=garden
        [AllowAnonymous]
        public virtual ViewResult PhotoDetails(int id, string photoName = null, string tag = null)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var photo = base.Repository.FindPhoto(id, allowPrivate);

            if (photo != null)
                return View(Views.PhotoDetails, new PhotoDetailsViewModel(base.Repository, photo, allowPrivate, tag));
            else
                return InvokeHttp404();
        }

        // GET: /Photos/Create
        [HttpGet]
        public virtual ViewResult PhotoCreate()
        {
            var viewModel = new PhotoCreateViewModel();

            return View(Views.PhotoCreate, viewModel);
        }

        // POST: /Photos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult PhotoCreate(FormCollection collection)
        {
            var name = collection["Name"];
            var fileName = collection["FileName"];
            bool isPrivate = String.IsNullOrWhiteSpace(collection["IsPrivate"]) ? false : true;
            var tagNamesStr = collection["Tags"];

            var viewModel = new PhotoCreateViewModel();
            viewModel.Photo.Name = name;

            if (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(tagNamesStr))
            {
                viewModel.ErrorMessage = "All fields are required.";

                return View(Views.PhotoCreate, viewModel);
            }

            if (!fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                fileName = fileName + ".jpg";

            try
            {
                var newPhoto = new Photo() { Name = name };
                newPhoto.IsPrivate = isPrivate;

                var tagNames = SplitTagNames(tagNamesStr);

                PhotoHelper.MovePhoto(fileName, newPhoto);
                PhotoHelper.GeneratePhotoSizes(newPhoto);

                base.Repository.SavePhoto(newPhoto, tagNames);

                CacheHelper.Insert("LastTagsInput", tagNamesStr);

                return RedirectToAction(Actions.PhotoDetails(newPhoto.ID));
            }
            catch (PhotoAlreadyExistsException ex)
            {
                viewModel.ErrorMessage = "Photo already exists at path: " + ex.Path;

                return View(Views.PhotoCreate, viewModel);
            }
        }

        // GET: /Photos/Edit/5
        public virtual ViewResult PhotoEdit(int id)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var photo = base.Repository.FindPhoto(id, allowPrivate);

            if (photo != null)
                return View(Views.PhotoEdit, new PhotoDetailsViewModel(base.Repository, photo, allowPrivate));
            else
                return InvokeHttp404();
        }

        // POST: /Photos/Edit/5
        [HttpPost]
        public virtual ActionResult PhotoEdit(int id, FormCollection collection)
        {
            var name = collection["Name"];
            bool isPrivate = String.IsNullOrWhiteSpace(collection["IsPrivate"]) ? false : true;
            var tagNamesStr = collection["Tags"];

            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var photo = base.Repository.FindPhoto(id, allowPrivate);
            if (photo != null)
            {
                photo.Name = name;
                photo.IsPrivate = isPrivate;

                var tagNames = SplitTagNames(tagNamesStr);

                base.Repository.SavePhoto(photo, tagNames);

                return RedirectToAction(Actions.PhotoDetails(photo.ID));
            }
            else
            {
                return InvokeHttp404();
            }
        }

        // GET: /Photos/Delete/5
        public virtual ViewResult PhotoDelete(int id)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var photo = base.Repository.FindPhoto(id, allowPrivate);

            if (photo != null)
                return View(Views.PhotoDelete, new PhotoDetailsViewModel(base.Repository, photo, allowPrivate));
            else
                return InvokeHttp404();
        }

        // POST: /Photos/Delete/5
        [HttpPost]
        public virtual ActionResult PhotoDelete(int id, FormCollection collection)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var photo = base.Repository.FindPhoto(id, allowPrivate);

            if (photo != null)
            {
                base.Repository.DeletePhoto(photo);

                return RedirectToAction(Actions.PhotoIndex());
            }
            else
            {
                return InvokeHttp404();
            }
        }

        // GET: /Photos/1234/Flowers_Large.jpg?v=1234567890
        [OutputCache(CacheProfile = "Forever", Location = OutputCacheLocation.Downstream)]
        [AllowAnonymous]
        public virtual ActionResult PhotoImage(int id, string fileName)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var fileNameParts = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            PhotoSizes size;
            if (fileNameParts.Length == 0 || !Enum.TryParse<PhotoSizes>(fileNameParts[fileNameParts.Length - 1], true, out size))
                return InvokeHttp404();

            var photo = base.Repository.FindPhoto(id, allowPrivate);
            if (photo != null)
            {
                var bytes = PhotoHelper.GetPhotoBytes(photo, size);

                if (bytes != null)
                    return File(bytes, "image/jpeg");
                else
                    return InvokeHttp404();
            }
            else
            {
                return InvokeHttp404();
            }
        }

        // GET: /Photos/1234/Download/Flowers_Large.jpg
        [AllowAnonymous]
        public virtual ActionResult PhotoImageDownload(int id, string fileName)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var fileNameParts = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            PhotoSizes size;
            if (fileNameParts.Length == 0 || !Enum.TryParse<PhotoSizes>(fileNameParts[fileNameParts.Length - 1], true, out size))
                return InvokeHttp404();

            var photo = base.Repository.FindPhoto(id, allowPrivate);
            if (photo != null)
            {
                var bytes = PhotoHelper.GetPhotoBytes(photo, size);

                if (bytes != null)
                    return File(bytes, "image/jpeg", fileName + ".jpg");
                else
                    return InvokeHttp404();
            }
            else
            {
                return InvokeHttp404();
            }
        }


        private List<string> SplitTagNames(string tagNames)
        {
            if (String.IsNullOrWhiteSpace(tagNames))
                return null;

            return tagNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(tagName => tagName.Trim())
                           .Distinct()
                           .ToList();
        }
    }
}
