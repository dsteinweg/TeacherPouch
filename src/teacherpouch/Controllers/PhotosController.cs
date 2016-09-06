using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Services;
using TeacherPouch.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;

namespace TeacherPouch.Controllers
{
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public class PhotosController : BaseController
    {
        public PhotosController(
            PhotoService photoService,
            TagService tagService,
            IMemoryCache cache,
            UserManager<ApplicationUser> userManager)
        {
            _photoService = photoService;
            _tagService = tagService;
            _cache = cache;
            _userManager = userManager;
        }

        private readonly PhotoService _photoService;
        private readonly TagService _tagService;
        private readonly IMemoryCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;

        [HttpGet("PhotoIndex")]
        [AllowAnonymous]
        public ViewResult PhotoIndex()
        {
            var photos = _photoService.GetAllPhotos();

            return View(photos);
        }

        [HttpGet("Photos/{id:int}/{photoName?}")]
        [AllowAnonymous]
        public ViewResult PhotoDetails(int id, string tag = null, string tag2 = null)
        {
            // TODO: extract this into service
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            var photoUrl = Url.Action(nameof(PhotoImage), new { id, fileName = photo.Name + ".jpg" });

            var smallFileSize = _photoService.GetPhotoFileSize(photo, PhotoSizes.Small);
            var largeFileSize = _photoService.GetPhotoFileSize(photo, PhotoSizes.Large);

            Tag firstTag = null;
            var firstTagPhotos = Enumerable.Empty<Photo>();
            if (!String.IsNullOrWhiteSpace(tag))
            {
                firstTag = _tagService.FindTag(tag);
                if (firstTag != null)
                    firstTagPhotos = firstTag.PhotoTags.Select(photoTag => _photoService.FindPhoto(photoTag.PhotoId));
            }

            Tag secondTag = null;
            var secondTagPhotos = Enumerable.Empty<Photo>();
            if (!String.IsNullOrWhiteSpace(tag2))
            {
                secondTag = _tagService.FindTag(tag2);
                if (secondTag != null)
                    secondTagPhotos = secondTag.PhotoTags.Select(photoTag => _photoService.FindPhoto(photoTag.PhotoId));
            }

            var allTagPhotos = secondTagPhotos.Any() ?
                firstTagPhotos.Intersect(secondTagPhotos).Distinct().ToList() :
                firstTagPhotos.ToList();

            var photoIndexInPhotosList = allTagPhotos.IndexOf(photo);

            var previousPhoto = allTagPhotos.ElementAtOrDefault(photoIndexInPhotosList - 1);
            if (previousPhoto == null && allTagPhotos.Count() > 1)
                previousPhoto = allTagPhotos.ElementAtOrDefault(allTagPhotos.Count() - 1);

            var nextPhoto = allTagPhotos.ElementAtOrDefault(photoIndexInPhotosList + 1);
            if (nextPhoto == null && allTagPhotos.Count() > 1)
                nextPhoto = allTagPhotos.ElementAtOrDefault(0);

            var userIsAdmin = User.IsInRole(TeacherPouchRoles.Admin);

            var viewModel = new PhotoDetailsViewModel(
                photo,
                photoUrl,
                smallFileSize,
                largeFileSize,
                firstTag,
                secondTag,
                previousPhoto,
                nextPhoto,
                userIsAdmin);

            return View(viewModel);
        }

        [HttpGet("Photos/Create")]
        public ViewResult PhotoCreate()
        {
            PhotoCreateViewModel viewModel;
            if (!_cache.TryGetValue<PhotoCreateViewModel>("NewPhotoViewModel", out viewModel))
                viewModel = new PhotoCreateViewModel(_photoService.PendingPhotoPath);

            return View(viewModel);
        }

        [HttpPost("Photos/Create")]
        [ValidateAntiForgeryToken]
        public ActionResult PhotoCreate(PhotoCreateViewModel postedViewModel)
        {
            postedViewModel.PendingPhotoPath = _photoService.PendingPhotoPath;

            if (String.IsNullOrWhiteSpace(postedViewModel.PhotoName) ||
                String.IsNullOrWhiteSpace(postedViewModel.FileName) ||
                String.IsNullOrWhiteSpace(postedViewModel.Tags))
            {
                postedViewModel.Message = "All fields are required.";

                return View(postedViewModel);
            }

            if (!postedViewModel.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                postedViewModel.FileName = postedViewModel.FileName + ".jpg";

            var newPhoto = new Photo
            {
                Name = postedViewModel.PhotoName,
                IsPrivate = postedViewModel.IsPrivate
            };

            _photoService.SavePhoto(newPhoto);
            _photoService.MovePhoto(postedViewModel.FileName, newPhoto);
            _photoService.GeneratePhotoSizes(newPhoto);

            var photoNameParts = postedViewModel.PhotoName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int photoNumber = 0;
            if (Int32.TryParse(photoNameParts.Last(), out photoNumber))
                photoNameParts[photoNameParts.Length - 1] = (photoNumber + 1).ToString();

            var newPhotoName = String.Join(" ", photoNameParts);
            var photoUrl = Url.Action(nameof(PhotoDetails), new { id = newPhoto.Id });

            var nextCreateViewModel = new PhotoCreateViewModel(_photoService.PendingPhotoPath);
            nextCreateViewModel.Message = $"<a href=\"{photoUrl}\">Photo \"{newPhoto.Name}\"</a> created.";
            nextCreateViewModel.Tags = postedViewModel.Tags;
            nextCreateViewModel.ProposedPhotoName = newPhotoName;

            _cache.Set("NewPhotoViewModel", nextCreateViewModel);

            return RedirectToAction(nameof(PhotoCreate));
        }

        [HttpGet("Photos/Edit/{id:int}")]
        public ViewResult PhotoEdit(int id)
        {
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            var viewModel = new PhotoEditViewModel
            {
                Name = photo.Name,
                IsPrivate = photo.IsPrivate,
                Tags = String.Join(
                    ", ",
                    photo.PhotoTags.Select(photoTag => _tagService.FindTag(photoTag.TagId)).Select(tag => tag.Name))
            };

            return View(viewModel);
        }

        [HttpPost("Photos/Edit/{id:int}")]
        public virtual ActionResult PhotoEdit(int id, PhotoEditViewModel postedViewModel)
        {
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            photo.Name = postedViewModel.Name;
            photo.IsPrivate = postedViewModel.IsPrivate;

            //var tagNames = SplitTagNames(postedViewModel.Tags);

            _photoService.SavePhoto(photo);

            return RedirectToAction(nameof(PhotoDetails), new { id = photo.Id });
        }

        [HttpGet("Photos/Delete/{id:int}")]
        public IActionResult PhotoDelete(int id)
        {
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            var tags = photo.PhotoTags.Select(photoTag => _tagService.FindTag(photoTag.TagId));

            var viewModel = new PhotoDeleteViewModel(photo, tags);

            return View(viewModel);
        }

        [HttpPost("Photos/Delete/{id:int}")]
        public ActionResult PhotoDeleteConfirmed(int id)
        {
            var photo =  _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            _photoService.DeletePhoto(photo);

            return RedirectToAction(nameof(PhotoIndex));
        }

        [HttpGet("Photos/{id:int}/{fileName}")]
        [AllowAnonymous]
        public IActionResult PhotoImage(int id, string fileName)
        {
            var fileNameParts = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            PhotoSizes size;
            if (fileNameParts.Length == 0 || !Enum.TryParse<PhotoSizes>(fileNameParts[fileNameParts.Length - 1], true, out size))
                return InvokeHttp404();

            var photo = _photoService.FindPhoto(id);
            if (photo != null)
                return InvokeHttp404();

            var bytes = _photoService.GetPhotoBytes(photo, size);
            if (bytes == null || bytes.Length == 0)
                return InvokeHttp404();

            return File(bytes, "image/jpeg");
        }

        [HttpGet("Photos/{id:int}/Download/{fileName}")]
        [AllowAnonymous]
        public IActionResult PhotoImageDownload(int id, string fileName)
        {
            var fileNameParts = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            PhotoSizes size;
            if (fileNameParts.Length == 0 || !Enum.TryParse<PhotoSizes>(fileNameParts[fileNameParts.Length - 1], true, out size))
                return InvokeHttp404();

            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            var bytes = _photoService.GetPhotoBytes(photo, size);
            if (bytes != null || bytes.Length == 0)
                return InvokeHttp404();

            return File(bytes, "image/jpeg", fileName + ".jpg");
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
