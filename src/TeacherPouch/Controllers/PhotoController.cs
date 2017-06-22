using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Services;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers
{
    [Route("photos")]
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public class PhotoController : BaseController
    {
        public PhotoController(
            PhotoService photoService,
            TagService tagService,
            IMemoryCache cache,
            UserManager<IdentityUser> userManager)
        {
            _photoService = photoService;
            _tagService = tagService;
            _cache = cache;
            _userManager = userManager;
        }

        private readonly PhotoService _photoService;
        private readonly TagService _tagService;
        private readonly IMemoryCache _cache;
        private readonly UserManager<IdentityUser> _userManager;

        [HttpGet("index")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var photos = _photoService.GetAllPhotos();

            return View(photos);
        }

        [HttpGet("{id:int}/{name?}", Name = "photo-details")]
        [AllowAnonymous]
        public IActionResult Details(int id, string name = null, string tag = null, string tag2 = null)
        {
            // TODO: extract this into service
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            var photoUrl = Url.Action(
                nameof(Image),
                new
                {
                    id,
                    size = PhotoSizes.Large,
                    fileName = photo.Name + ".jpg"
                });

            var smallFileSize = _photoService.GetPhotoFileSize(photo, PhotoSizes.Small);
            var smallDownloadUrl = Url.Action(nameof(Download), new { id, size = PhotoSizes.Small, fileName = photo.Name });
            var largeFileSize = _photoService.GetPhotoFileSize(photo, PhotoSizes.Large);
            var largeDownloadUrl = Url.Action(nameof(Download), new { id, size = PhotoSizes.Large, fileName = photo.Name });

            Tag firstTag = null;
            var firstTagPhotos = Enumerable.Empty<Photo>();
            if (!String.IsNullOrWhiteSpace(tag))
            {
                firstTag = _tagService.FindTag(tag);
                if (firstTag != null)
                    firstTagPhotos = firstTag.PhotoTags.Select(pt => pt.Photo);
            }

            Tag secondTag = null;
            var secondTagPhotos = Enumerable.Empty<Photo>();
            if (!String.IsNullOrWhiteSpace(tag2))
            {
                secondTag = _tagService.FindTag(tag2);
                if (secondTag != null)
                    secondTagPhotos = secondTag.PhotoTags.Select(pt => pt.Photo);
            }

            var allTagPhotos = secondTagPhotos.Any()
                ? firstTagPhotos.Intersect(secondTagPhotos).Distinct().OrderBy(p => p.Id).ToList()
                : firstTagPhotos.OrderBy(p => p.Id).ToList();

            var photoIndexInPhotosList = allTagPhotos.IndexOf(photo);

            var previousPhoto = allTagPhotos.ElementAtOrDefault(photoIndexInPhotosList - 1);
            if (previousPhoto == null && allTagPhotos.Count() > 1)
                previousPhoto = allTagPhotos.LastOrDefault();

            var nextPhoto = allTagPhotos.ElementAtOrDefault(photoIndexInPhotosList + 1);
            if (nextPhoto == null && allTagPhotos.Count() > 1)
                nextPhoto = allTagPhotos.FirstOrDefault();

            var userIsAdmin = User.IsInRole(TeacherPouchRoles.Admin);

            var viewModel = new PhotoDetailsViewModel(
                photo,
                photoUrl,
                smallFileSize,
                smallDownloadUrl,
                largeFileSize,
                largeDownloadUrl,
                firstTag,
                secondTag,
                previousPhoto,
                nextPhoto,
                userIsAdmin);

            return View(viewModel);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            PhotoCreateViewModel viewModel;
            if (!_cache.TryGetValue<PhotoCreateViewModel>("New Photo View Model", out viewModel))
                viewModel = new PhotoCreateViewModel(_photoService.PendingPhotoPath);

            return View(viewModel);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhotoCreateViewModel postedViewModel)
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
            var photoUrl = Url.Action(nameof(Details), new { id = newPhoto.Id });

            var nextCreateViewModel = new PhotoCreateViewModel(_photoService.PendingPhotoPath);
            nextCreateViewModel.Message = $"<a href=\"{photoUrl}\">Photo \"{newPhoto.Name}\"</a> created.";
            nextCreateViewModel.Tags = postedViewModel.Tags;
            nextCreateViewModel.ProposedPhotoName = newPhotoName;

            _cache.Set("New Photo View Model", nextCreateViewModel);

            return RedirectToAction(nameof(Create));
        }

        [HttpGet("{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            var photoUrl = Url.Action(nameof(Image), new { id, size = PhotoSizes.Large, fileName = photo.Name });
            var tags = photo.PhotoTags.Select(pt => pt.Tag);

            var viewModel = new PhotoEditViewModel(photo, photoUrl, tags);

            return View(viewModel);
        }

        [HttpPost("{id:int}/edit")]
        public IActionResult Edit(int id, PhotoEditViewModel postedViewModel)
        {
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            photo.Name = postedViewModel.Name;
            photo.IsPrivate = postedViewModel.Private;

            //var tagNames = SplitTagNames(postedViewModel.Tags);

            _photoService.SavePhoto(photo);

            return RedirectToAction(nameof(Details), new { id = photo.Id });
        }

        [HttpGet("{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            var photoUrl = Url.Action(nameof(Image), new { id, size = PhotoSizes.Large, fileName = photo.Name });
            var tags = photo.PhotoTags.Select(photoTag => _tagService.FindTag(photoTag.TagId));
            var viewModel = new PhotoDeleteViewModel(photo, photoUrl, tags);

            return View(viewModel);
        }

        [HttpPost("{id:int}/delete")]
        public IActionResult Delete(int id, bool confirmed = true)
        {
            var photo =  _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            _photoService.DeletePhoto(photo);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("{id:int}/{size}/{fileName}")]
        [AllowAnonymous]
        public IActionResult Image(int id, PhotoSizes size, string fileName)
        {
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            var bytes = _photoService.GetPhotoBytes(photo, size);
            if (bytes == null || bytes.Length == 0)
                return InvokeHttp404();

            return File(bytes, "image/jpeg");
        }

        [HttpGet("{id:int}/{size}/download/{fileName}.jpg")]
        [AllowAnonymous]
        public IActionResult Download(int id, PhotoSizes size, string fileName)
        {
            var photo = _photoService.FindPhoto(id);
            if (photo == null)
                return InvokeHttp404();

            var bytes = _photoService.GetPhotoBytes(photo, size);
            if (bytes == null || bytes.Length == 0)
                return InvokeHttp404();

            return File(bytes, "image/jpeg", fileName + ".jpg");
        }

        private List<string> SplitTagNames(string tagNames)
        {
            if (String.IsNullOrWhiteSpace(tagNames))
                return null;

            return tagNames
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(tagName => tagName.Trim())
                .Distinct()
                .ToList();
        }
    }
}
