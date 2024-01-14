using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TeacherPouch.Models;
using TeacherPouch.Services;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers;

[Route("photos")]
public class PhotoController(PhotoService _photoService, TagService _tagService, IMemoryCache _cache) : Controller
{
    [HttpGet("index")]
    public async Task<IActionResult> Index(int? page)
    {
        var photos = await _photoService.GetPhotoIndexPage(page.GetValueOrDefault(1));

        return View(photos);
    }

    [HttpGet("{id:int}/{name?}", Name = "photo-details")]
    public async Task<IActionResult> Details(
        int id, string? name = null, string? tag = null, string? tag2 = null, CancellationToken cancellationToken = default)
    {
        var photo = await _photoService.FindPhoto(id, cancellationToken);
        if (photo is null)
            return NotFound();

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

        Tag? firstTag = null;
        var firstTagPhotos = Enumerable.Empty<Photo>();
        if (!string.IsNullOrWhiteSpace(tag))
        {
            firstTag = await _tagService.FindTag(tag, cancellationToken);
            if (firstTag is not null)
                firstTagPhotos = firstTag.PhotoTags.Select(pt => pt.Photo);
        }

        Tag? secondTag = null;
        var secondTagPhotos = Enumerable.Empty<Photo>();
        if (!string.IsNullOrWhiteSpace(tag2))
        {
            secondTag = await _tagService.FindTag(tag2, cancellationToken);
            if (secondTag is not null)
                secondTagPhotos = secondTag.PhotoTags.Select(pt => pt.Photo);
        }

        var allTagPhotos = secondTagPhotos.Any()
            ? firstTagPhotos.Intersect(secondTagPhotos).Distinct().OrderBy(p => p.Id).ToList()
            : firstTagPhotos.OrderBy(p => p.Id).ToList();

        var photoIndexInPhotosList = allTagPhotos.IndexOf(photo);

        var previousPhoto = allTagPhotos.ElementAtOrDefault(photoIndexInPhotosList - 1);
        if (previousPhoto is null && allTagPhotos.Count > 1)
            previousPhoto = allTagPhotos.LastOrDefault();

        var nextPhoto = allTagPhotos.ElementAtOrDefault(photoIndexInPhotosList + 1);
        if (nextPhoto is null && allTagPhotos.Count > 1)
            nextPhoto = allTagPhotos.FirstOrDefault();

        var userIsAdmin = User.IsInRole(TeacherPouchRoles.Admin);

        var viewModel = new PhotoDetailsViewModel(
            photo,
            photoUrl!,
            smallFileSize,
            smallDownloadUrl!,
            largeFileSize,
            largeDownloadUrl!,
            firstTag!,
            secondTag,
            previousPhoto,
            nextPhoto,
            userIsAdmin);

        return View(viewModel);
    }

    [HttpGet("create")]
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public IActionResult Create()
    {
        if (!_cache.TryGetValue<PhotoCreateViewModel>("New Photo View Model", out var viewModel))
            viewModel = new PhotoCreateViewModel(_photoService.PendingPhotoPath);

        return View(viewModel);
    }

    [HttpPost("create")]
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public async Task<IActionResult> Create(PhotoCreateViewModel postedViewModel, CancellationToken cancellationToken)
    {
        postedViewModel.PendingPhotoPath = _photoService.PendingPhotoPath;

        if (string.IsNullOrWhiteSpace(postedViewModel.PhotoName) ||
            string.IsNullOrWhiteSpace(postedViewModel.FileName) ||
            string.IsNullOrWhiteSpace(postedViewModel.Tags))
        {
            postedViewModel.Message = "All fields are required.";

            return View(postedViewModel);
        }

        if (!postedViewModel.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            postedViewModel.FileName += ".jpg";

        var newPhoto = new Photo
        {
            Name = postedViewModel.PhotoName,
            IsPrivate = postedViewModel.IsPrivate
        };

        _photoService.SavePhoto(newPhoto);
        _photoService.MovePhoto(postedViewModel.FileName, newPhoto);
        await _photoService.GeneratePhotoSizes(newPhoto, cancellationToken);

        var photoNameParts = postedViewModel.PhotoName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (int.TryParse(photoNameParts.Last(), out var photoNumber))
            photoNameParts[^1] = (photoNumber + 1).ToString();

        var newPhotoName = string.Join(" ", photoNameParts);
        var photoUrl = Url.Action(nameof(Details), new { id = newPhoto.Id });

        var nextCreateViewModel = new PhotoCreateViewModel(_photoService.PendingPhotoPath)
        {
            Message = $"<a href=\"{photoUrl}\">Photo \"{newPhoto.Name}\"</a> created.",
            Tags = postedViewModel.Tags,
            ProposedPhotoName = newPhotoName
        };

        _ = _cache.Set("New Photo View Model", nextCreateViewModel);

        return RedirectToAction(nameof(Create));
    }

    [HttpGet("{id:int}/edit")]
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var photo = await _photoService.FindPhoto(id, cancellationToken);
        if (photo is null)
            return NotFound();

        var photoUrl = Url.Action(nameof(Image), new { id, size = PhotoSizes.Large, fileName = photo.Name });
        var tags = photo.PhotoTags.Select(pt => pt.Tag);

        var viewModel = new PhotoEditViewModel(photo, photoUrl!, tags);

        return View(viewModel);
    }

    [HttpPost("{id:int}/edit")]
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public async Task<IActionResult> Edit(int id, PhotoEditViewModel postedViewModel, CancellationToken cancellationToken)
    {
        var photo = await _photoService.FindPhoto(id, cancellationToken);
        if (photo is null)
            return NotFound();

        if (!ModelState.IsValid)
            return View();

        photo.Name = postedViewModel.Name!;
        photo.IsPrivate = postedViewModel.Private;

        _photoService.SavePhoto(photo);

        return RedirectToAction(nameof(Details), new { id = photo.Id });
    }

    [HttpGet("{id:int}/delete")]
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var photo = await _photoService.FindPhoto(id, cancellationToken);
        if (photo is null)
            return NotFound();

        var photoUrl = Url.Action(nameof(Image), new { id, size = PhotoSizes.Large, fileName = photo.Name });
        var tagsTasks = photo.PhotoTags.Select(async photoTag => (await _tagService.FindTag(photoTag.TagId))!);
        var tags = await Task.WhenAll(tagsTasks);
        var viewModel = new PhotoDeleteViewModel(photo, photoUrl!, tags);

        return View(viewModel);
    }

    [HttpPost("{id:int}/delete")]
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public async Task<IActionResult> Delete(int id, bool confirmed = true, CancellationToken cancellationToken = default)
    {
        var photo = await _photoService.FindPhoto(id, cancellationToken);
        if (photo is null)
            return NotFound();

        _photoService.DeletePhoto(photo);

        return RedirectToAction(nameof(Index));
    }

#pragma warning disable IDE0060 // Remove unused parameter
    [HttpGet("{id:int}/{size}/{fileName}")]
    public async Task<IActionResult> Image(int id, PhotoSizes size, string fileName, CancellationToken cancellationToken)
    {
        var photo = await _photoService.FindPhoto(id, cancellationToken);
        if (photo is null)
            return NotFound();

        var bytes = await _photoService.GetPhotoBytes(photo, size, cancellationToken);
        if (bytes is null || bytes.Length == 0)
            return NotFound();

        return File(bytes, MediaTypeNames.Image.Jpeg);
    }
#pragma warning restore IDE0060 // Remove unused parameter

    [HttpGet("{id:int}/{size}/download/{fileName}.jpg")]
    public async Task<IActionResult> Download(int id, PhotoSizes size, string fileName, CancellationToken cancellationToken)
    {
        var photo = await _photoService.FindPhoto(id, cancellationToken);
        if (photo is null)
            return NotFound();

        var bytes = await _photoService.GetPhotoBytes(photo, size, cancellationToken);
        if (bytes is null || bytes.Length == 0)
            return NotFound();

        return File(bytes, MediaTypeNames.Image.Jpeg, fileName + ".jpg");
    }

    private static List<string> SplitTagNames(string tagNames)
    {
        if (string.IsNullOrWhiteSpace(tagNames))
            return [];

        return tagNames
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(tagName => tagName.Trim())
            .Distinct()
            .ToList();
    }
}
