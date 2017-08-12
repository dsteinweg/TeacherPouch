using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Services;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers
{
    [Route("tags")]
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public class TagController : BaseController
    {
        public TagController(
            TagService tagService,
            PhotoService photoService,
            SearchService searchService)
        {
            _tagService = tagService;
            _photoService = photoService;
            _searchService = searchService;
        }

        private readonly TagService _tagService;
        private readonly PhotoService _photoService;
        private readonly SearchService _searchService;

        [HttpGet("~/api/tags")]
        public IEnumerable<string> Get(string q)
        {
            return _searchService.TagAutocompleteSearch(q);
        }

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var allTags = _tagService.GetAllTags().OrderBy(tag => tag.Name);

            return View(allTags);
        }

        [HttpGet("{id:int}/{name?}")]
        [AllowAnonymous]
        public IActionResult Details(int id, string name)
        {
            var tag = _tagService.FindTag(id);
            if (tag == null)
                return InvokeHttp404();

            var viewModel = new TagDetailsViewModel(
                tag,
                tag.PhotoTags.Select(pt => pt.Photo));

            return View(viewModel);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            var viewModel = new TagCreateViewModel();

            return View(viewModel);
        }

        [HttpPost("create")]
        public IActionResult Create(TagCreateViewModel postedViewModel)
        {
            if (String.IsNullOrWhiteSpace(postedViewModel.TagName))
            {
                postedViewModel.ErrorMessage = "All fields are required.";

                return View(postedViewModel);
            }

            var tag = new Tag()
            {
                Name = postedViewModel.TagName,
                IsPrivate = postedViewModel.Private
            };

            _tagService.SaveTag(tag);

            return RedirectToAction(nameof(Details), new { id = tag.Id, name = tag.Name });
        }

        [HttpGet("{id:int}/Edit")]
        public IActionResult Edit(int id)
        {
            var tag = _tagService.FindTag(id);
            if (tag == null)
                return InvokeHttp404();

            var viewModel = new TagEditViewModel(tag);

            return View(viewModel);
        }

        [HttpPost("{id:int}/Edit")]
        public IActionResult Edit(int id, TagEditViewModel postedViewModel)
        {
            var tag = _tagService.FindTag(id);
            if (tag == null)
                return InvokeHttp404();

            var existingTag = _tagService.FindTag(postedViewModel.Name);
            if (existingTag != null)
                ModelState.AddModelError("Duplicate tag", "Tag exists with that name.");

            if (!ModelState.IsValid)
            {
                var viewModel = new TagEditViewModel(tag);

                return View(viewModel);
            }

            tag.Name = postedViewModel.Name;
            tag.IsPrivate = postedViewModel.IsPrivate;

            _tagService.SaveTag(tag);

            return RedirectToAction(nameof(Details), new { id = tag.Id });
        }

        [HttpGet("{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var tag = _tagService.FindTag(id);
            if (tag == null)
                return InvokeHttp404();

            return View(tag);
        }

        [HttpPost("{id:int}/delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var tag = _tagService.FindTag(id);
            if (tag == null)
                return InvokeHttp404();

            _tagService.DeleteTag(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
