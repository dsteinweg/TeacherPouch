﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using TeacherPouch.Services;

namespace TeacherPouch.Controllers
{
    [Route("tags")]
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public class TagsController : BaseController
    {
        public TagsController(TagService tagService, PhotoService photoService)
        {
            _tagService = tagService;
            _photoService = photoService;
        }

        private readonly TagService _tagService;
        private readonly PhotoService _photoService;

        [HttpGet("")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var allTags = _tagService.GetAllTags().OrderBy(tag => tag.Name);

            return View(allTags);
        }

        [HttpGet("{name}")]
        [AllowAnonymous]
        public IActionResult TagDetails(string name)
        {
            var tag = _tagService.FindTag(name);
            if (tag != null)
                return InvokeHttp404();

            var viewModel = new TagDetailsViewModel(tag);

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
                IsPrivate = postedViewModel.IsPrivate
            };

            _tagService.SaveTag(tag);

            return RedirectToAction(nameof(TagDetails), new { tagName = tag.Name} );
        }

        [HttpGet("{id:int}/Edit")]
        public IActionResult TagEdit(int id)
        {
            var tag = _tagService.FindTag(id);

            if (tag == null)
                return InvokeHttp404();

            var viewModel = new TagEditViewModel(tag);

            return View(tag);
        }

        [HttpPost("{id:int}/Edit")]
        public IActionResult TagEdit(int id, TagEditViewModel postedViewModel)
        {
            var tag = _tagService.FindTag(id);
            if (tag == null)
                return InvokeHttp404();

            if (String.IsNullOrWhiteSpace(postedViewModel.TagName))
            {
                postedViewModel.ErrorMessage = "Tag must have a name.";

                return View(postedViewModel);
            }

            var existingTag = _tagService.FindTag(postedViewModel.TagName);
            if (existingTag != null)
            {
                postedViewModel.ErrorMessage = "Tag exists with that name.";

                return View(postedViewModel);
            }

            tag.Name = postedViewModel.TagName;
            tag.IsPrivate = postedViewModel.IsPrivate;

            _tagService.SaveTag(tag);

            return RedirectToAction(nameof(TagDetails), new { id = tag.Id });
        }

        [HttpGet("{id:int}/delete")]
        public IActionResult TagDelete(int id)
        {
            var tag = _tagService.FindTag(id);
            if (tag == null)
                return InvokeHttp404();

            return View(tag);
        }

        [HttpPost("{id:int}/delete")]
        public IActionResult TagDeleteConfirmed(int id)
        {
            var tag = _tagService.FindTag(id);

            if (tag == null)
                return InvokeHttp404();

            _tagService.DeleteTag(tag);

            return RedirectToAction(nameof(Index));
        }
    }
}