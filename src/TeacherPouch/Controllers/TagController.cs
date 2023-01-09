﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Services;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers;

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
    public async Task<ActionResult<string[]>> Get(string q, CancellationToken cancellationToken)
    {
        return await _searchService.TagAutocompleteSearch(q, cancellationToken);
    }

    [HttpGet("")]
    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var allTags = await _tagService.GetAllTags(cancellationToken);
        return View(allTags.OrderBy(tag => tag.Name));
    }

    [HttpGet("{id:int}/{name?}")]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, string name, CancellationToken cancellationToken)
    {
        var tag = await _tagService.FindTag(id, cancellationToken);
        if (tag is null)
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
    public async Task<IActionResult> Create(TagCreateViewModel postedViewModel, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(postedViewModel.TagName))
        {
            postedViewModel.ErrorMessage = "All fields are required.";
            return View(postedViewModel);
        }

        var tag = new Tag
        {
            Name = postedViewModel.TagName,
            IsPrivate = postedViewModel.Private
        };

        await _tagService.SaveTag(tag, cancellationToken);

        return RedirectToAction(nameof(Details), new { id = tag.Id, name = tag.Name });
    }

    [HttpGet("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var tag = await _tagService.FindTag(id, cancellationToken);
        if (tag is null)
            return InvokeHttp404();

        var viewModel = new TagEditViewModel(tag);

        return View(viewModel);
    }

    [HttpPost("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id, TagEditViewModel postedViewModel, CancellationToken cancellationToken)
    {
        var tag = await _tagService.FindTag(id, cancellationToken);
        if (tag is null)
            return InvokeHttp404();

        if (!string.IsNullOrWhiteSpace(postedViewModel.Name))
        {
            var existingTag = await _tagService.FindTag(postedViewModel.Name, cancellationToken);
            if (existingTag is not null)
                ModelState.AddModelError("Duplicate tag", "Tag exists with that name.");
        }

        if (!ModelState.IsValid)
        {
            var viewModel = new TagEditViewModel(tag);
            return View(viewModel);
        }

        tag.Name = postedViewModel.Name!;
        tag.IsPrivate = postedViewModel.IsPrivate;

        await _tagService.SaveTag(tag, cancellationToken);

        return RedirectToAction(nameof(Details), new { id = tag.Id });
    }

    [HttpGet("{id:int}/delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var tag = await _tagService.FindTag(id, cancellationToken);
        if (tag is null)
            return InvokeHttp404();

        return View(tag);
    }

    [HttpPost("{id:int}/delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var tag = await _tagService.FindTag(id, cancellationToken);
        if (tag is null)
            return InvokeHttp404();

        await _tagService.DeleteTag(id, cancellationToken);

        return RedirectToAction(nameof(Index));
    }
}
