using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Data;
using TeacherPouch.Helpers;
using TeacherPouch.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;

namespace TeacherPouch.Controllers
{
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public class PhotosController : BaseController
    {
        public PhotosController(
            IRepository repository,
            IMemoryCache cache,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _cache = cache;
            _userManager = userManager;
        }

        private readonly IRepository _repository;
        private readonly IMemoryCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;

        [HttpGet("PhotoIndex")]
        [AllowAnonymous]
        public ViewResult PhotoIndex()
        {
            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(User);

            var photos = _repository.GetAllPhotos(allowPrivate).OrderBy(photo => photo.ID);

            return View(photos);
        }

        [HttpGet("Photos/{id:int}/{photoName?}")]
        [AllowAnonymous]
        public ViewResult PhotoDetails(int id, string photoName = null, string tag = null, string tag2 = null)
        {
            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(User);
            var photo = _repository.FindPhoto(id, allowPrivate);

            if (photo == null)
                return InvokeHttp404();

            var userIsAdmin = SecurityHelper.UserIsAdmin(User);
            var viewModel = new PhotoDetailsViewModel(_repository, photo, allowPrivate, userIsAdmin, tag, tag2);

            return View(viewModel);
        }

        [HttpGet("Photos/Create")]
        public ViewResult PhotoCreate()
        {
            PhotoCreateViewModel viewModel; 
            if (!_cache.TryGetValue<PhotoCreateViewModel>("NewPhotoViewModel", out viewModel))
                viewModel = new PhotoCreateViewModel();

            return View(viewModel);
        }

        [HttpPost("Photos/Create")]
        [ValidateAntiForgeryToken]
        public ActionResult PhotoCreate(PhotoCreateViewModel postedViewModel)
        {
            //todo finishdhd so tired

            var name = collection["Name"];
            var fileName = collection["FileName"];
            bool isPrivate = String.IsNullOrWhiteSpace(collection["IsPrivate"]) ? false : true;
            var tagNamesStr = collection["Tags"];

            var viewModel = new PhotoCreateViewModel();
            viewModel.Photo.Name = name;

            if (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(tagNamesStr))
            {
                viewModel.ErrorMessage = "All fields are required.";

                return View(viewModel);
            }

            if (!fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                fileName = fileName + ".jpg";

            var newPhoto = new Photo() { Name = name };
            newPhoto.IsPrivate = isPrivate;

            var tagNames = SplitTagNames(tagNamesStr);

            PhotoHelper.MovePhoto(fileName, newPhoto);
            PhotoHelper.GeneratePhotoSizes(newPhoto);

            _repository.SavePhoto(newPhoto, tagNames);

            var photoNameParts = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int photoNumber = 0;
            if (Int32.TryParse(photoNameParts.Last(), out photoNumber))
            {
                photoNameParts[photoNameParts.Length - 1] = (photoNumber + 1).ToString();
            }
 
            var newPhotoName = String.Join(" ", photoNameParts);
            var photoUrl = Url.Action(nameof(PhotoDetails), new { id = newPhoto.ID });

            var nextCreateViewModel = new PhotoCreateViewModel();
            nextCreateViewModel.Message = $"<a href=\"{photoUrl}\">Photo \"{newPhoto.Name}\"</a> created.";
            nextCreateViewModel.LastTagsInput = tagNamesStr;
            nextCreateViewModel.ProposedPhotoName = newPhotoName;

            _cache.Set("NewPhotoViewModel", nextCreateViewModel);

            return RedirectToAction(nameof(PhotoCreate));
        }

        [HttpGet("Photos/Edit/{id:int}")]
        public ViewResult PhotoEdit(int id)
        {
            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(User);
            var photo = _repository.FindPhoto(id, allowPrivate);

            if (photo == null)
                return InvokeHttp404();

            var userIsAdmin = SecurityHelper.UserIsAdmin(User);
            return View(new PhotoDetailsViewModel(_repository, photo, allowPrivate, userIsAdmin));
        }

        [HttpPost("Photos/Edit/{id:int}")]
        public virtual ActionResult PhotoEdit(int id, PhotoEditViewModel postedViewModel)
        {
            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(User);

            var photo = _repository.FindPhoto(id, allowPrivate);
            if (photo != null)
            {
                photo.Name = postedViewModel.Name;
                photo.IsPrivate = postedViewModel.IsPrivate;

                var tagNames = SplitTagNames(postedViewModel.Tags);

                _repository.SavePhoto(photo, tagNames);

                return RedirectToAction(nameof(PhotoDetails), new { id = photo.ID });
            }
            else
            {
                return InvokeHttp404();
            }
        }

        [HttpGet("Photos/Delete/{id:int}")]
        public ViewResult PhotoDelete(int id)
        {
            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(User);

            var photo = _repository.FindPhoto(id, allowPrivate);
            if (photo == null)
                return InvokeHttp404();

            var userIsAdmin = SecurityHelper.UserIsAdmin(User);

            return View(new PhotoDetailsViewModel(_repository, photo, allowPrivate, userIsAdmin));
        }

        [HttpPost("Photos/Delete/{id:int}")]
        public ActionResult PhotoDeleteConfirmed(int id)
        {
            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(User);

            var photo = _repository.FindPhoto(id, allowPrivate);

            if (photo != null)
            {
                _repository.DeletePhoto(photo);

                return RedirectToAction(nameof(PhotoIndex));
            }
            else
            {
                return InvokeHttp404();
            }
        }

        //[OutputCache(CacheProfile = "Forever", Location = OutputCacheLocation.Downstream)]
        [HttpGet("Photos/{id:int}/{fileName}")]
        [AllowAnonymous]
        public virtual ActionResult PhotoImage(int id, string fileName)
        {
            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(User);

            var fileNameParts = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            PhotoSizes size;
            if (fileNameParts.Length == 0 || !Enum.TryParse<PhotoSizes>(fileNameParts[fileNameParts.Length - 1], true, out size))
                return InvokeHttp404();

            var photo =_repository.FindPhoto(id, allowPrivate);
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

        [HttpGet("Photos/{id:int}/Download/{fileName}")]
        [AllowAnonymous]
        public virtual ActionResult PhotoImageDownload(int id, string fileName)
        {
            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(User);

            var fileNameParts = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            PhotoSizes size;
            if (fileNameParts.Length == 0 || !Enum.TryParse<PhotoSizes>(fileNameParts[fileNameParts.Length - 1], true, out size))
                return InvokeHttp404();

            var photo = _repository.FindPhoto(id, allowPrivate);
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

            return tagNames
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(tagName => tagName.Trim())
                .Distinct()
                .ToList();
        }
    }
}
