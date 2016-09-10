using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using TeacherPouch.Services;

namespace TeacherPouch.Controllers
{
    [Route("Tags")]
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
        public ViewResult TagIndex()
        {
            var allTags = _tagService.GetAllTags().OrderBy(tag => tag.Name);

            return View(allTags);
        }

        [HttpGet("{name}")]
        [AllowAnonymous]
        public ViewResult TagDetails(string name)
        {
            var tag = _tagService.FindTag(name);
            if (tag != null)
                return InvokeHttp404();

            var viewModel = new TagDetailsViewModel(tag);

            return View(viewModel);
        }

        [HttpGet("CreateNew")]
        public ViewResult TagCreate()
        {
            var viewModel = new TagCreateViewModel();

            return View(viewModel);
        }

        [HttpPost("CreateNew")]
        public IActionResult TagCreate(TagCreateViewModel postedViewModel)
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

        [HttpGet("Edit/{id:int}")]
        public ViewResult TagEdit(int id)
        {
            var tag = _tagService.FindTag(id);

            if (tag == null)
                return InvokeHttp404();

            var viewModel = new TagEditViewModel(tag);

            return View(tag);
        }

        [HttpPost("Edit/{id:int}")]
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

        [HttpGet("Delete/{id:int}")]
        public ViewResult TagDelete(int id)
        {
            var tag = _tagService.FindTag(id);
            if (tag == null)
                return InvokeHttp404();

            return View(tag);
        }

        [HttpPost("Delete/{id:int}")]
        public IActionResult TagDeleteConfirmed(int id)
        {
            var tag = _tagService.FindTag(id);

            if (tag == null)
                return InvokeHttp404();

            _tagService.DeleteTag(tag);

            return RedirectToAction(nameof(TagIndex));
        }
    }
}
