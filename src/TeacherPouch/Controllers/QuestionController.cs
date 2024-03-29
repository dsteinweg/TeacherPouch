﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeacherPouch.Data;
using TeacherPouch.Models;
using TeacherPouch.Services;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers;

[Authorize(Roles = TeacherPouchRoles.Admin)]
public class QuestionController(TeacherPouchDbContext _dbContext, PhotoService _photoService) : Controller
{
    [HttpGet("questions")]
    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = new QuestionIndexViewModel
        {
            Questions = await _dbContext.Questions.ToArrayAsync(cancellationToken),
            DisplayAdminLinks = User.IsInRole(TeacherPouchRoles.Admin)
        };

        return View(viewModel);
    }

    [HttpGet("questions/{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        if (question is null)
            return NotFound();

        var photo = await _photoService.FindPhoto(question.PhotoId, cancellationToken);
        if (photo is null)
            return NotFound();

        var viewModel = new QuestionDetailsViewModel(question, photo)
        {
            DisplayAdminLinks = User.IsInRole(TeacherPouchRoles.Admin)
        };

        return View(viewModel);
    }

    [HttpGet("questions/create")]
    public async Task<IActionResult> Create(int photoId, CancellationToken cancellationToken)
    {
        var photo = await _photoService.FindPhoto(photoId, cancellationToken);
        if (photo is null)
            return NotFound();

        var viewModel = new QuestionCreateViewModel(photo);

        return View(viewModel);
    }

    [HttpPost("questions/create")]
    public async Task<IActionResult> Create(int photoId, QuestionCreateViewModel postedViewModel, CancellationToken cancellationToken)
    {
        var photo = await _photoService.FindPhoto(photoId, cancellationToken);
        if (photo is null)
            return NotFound();

        if (!ModelState.IsValid)
            return View(postedViewModel);

        var question = new Question
        {
            PhotoId = photoId,
            Text = postedViewModel.QuestionText!,
            SentenceStarters = postedViewModel.QuestionSentenceStarters!,
            Order = postedViewModel.QuestionOrder
        };

        _ = _dbContext.Questions.Add(question);
        _ = await _dbContext.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Details), new { id = question.Id });
    }

    [HttpGet("questions/{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        if (question is null)
            return NotFound();

        var photo = await _photoService.FindPhoto(question.PhotoId, cancellationToken);
        if (photo is null)
            return NotFound();

        var viewModel = new QuestionEditViewModel(question, photo);

        return View(viewModel);
    }

    [HttpPost("questions/{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, QuestionEditViewModel postedViewModel, CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        if (question is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            var photo = await _photoService.FindPhoto(question.PhotoId, cancellationToken);
            if (photo is null)
                return NotFound();

            var viewModel = new QuestionEditViewModel(question, photo);

            return View(viewModel);
        }

        question.Text = postedViewModel.QuestionText!;
        question.SentenceStarters = postedViewModel.QuestionSentenceStarters;
        question.Order = postedViewModel.QuestionOrder;

        _ = _dbContext.Questions.Update(question);
        _ = await _dbContext.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("questions/{id:int}/delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        if (question is null)
            return NotFound();

        var photo = await _photoService.FindPhoto(question.PhotoId, cancellationToken);
        if (photo is null)
            return NotFound();

        var viewModel = new QuestionDetailsViewModel(question, photo);

        return View(viewModel);
    }

    [HttpPost("questions/{id:int}/delete")]
    public async Task<IActionResult> Delete(int id, QuestionDetailsViewModel postedViewModel, CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        if (question is null)
            return NotFound();

        _ = _dbContext.Remove(question);
        _ = await _dbContext.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Index));
    }
}
