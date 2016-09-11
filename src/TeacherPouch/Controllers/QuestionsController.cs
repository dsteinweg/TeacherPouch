using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Data;
using TeacherPouch.Models;
using TeacherPouch.Services;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers
{
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public class QuestionsController : BaseController
    {
        public QuestionsController(
            TeacherPouchDbContext dbContext,
            PhotoService photoService,
            TagService tagService)
        {
            _db = dbContext;
            _photoService = photoService;
            _tagService = tagService;
        }

        private readonly TeacherPouchDbContext _db;
        private readonly PhotoService _photoService;
        private readonly TagService _tagService;

        [HttpGet("questions")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var viewModel = new QuestionIndexViewModel();
            viewModel.Questions = _db.Questions.ToList();
            viewModel.DisplayAdminLinks = User.IsInRole(TeacherPouchRoles.Admin);

            return View(viewModel);
        }

        [HttpGet("questions/{id:int}")]
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var question = _db.Questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
                return InvokeHttp404();

            var photo = _photoService.FindPhoto(question.PhotoId);
            if (photo == null)
                return InvokeHttp404();

            var viewModel = new QuestionDetailsViewModel(question, photo)
            {
                DisplayAdminLinks = User.IsInRole(TeacherPouchRoles.Admin)
            };

            return View(viewModel);
        }

        [HttpGet("photos/{photoId:int}/questions")]
        [AllowAnonymous]
        public IActionResult QuestionsForPhoto(int photoId)
        {
            return View();
        }

        [HttpGet("photos/{photoId:int}/questions/create")]
        public IActionResult Create(int photoId)
        {
            var photo = _photoService.FindPhoto(photoId);
            if (photo == null)
                return InvokeHttp404();

            var viewModel = new QuestionCreateViewModel(photo);

            return View(viewModel);
        }

        [HttpPost("photos/{photoId:int}/questions/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int photoId, QuestionCreateViewModel postedViewModel)
        {
            var photo = _photoService.FindPhoto(photoId);
            if (photo == null)
                return InvokeHttp404();

            if (!ModelState.IsValid)
                return View(postedViewModel);

            var question = new Question
            {
                PhotoId = photoId,
                Text = postedViewModel.QuestionText,
                SentenceStarters = postedViewModel.QuestionSentenceStarters,
                Order = postedViewModel.QuestionOrder
            };

            _db.Questions.Add(question);
            _db.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = question.Id });
        }

        [HttpGet("questions/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var question = _db.Questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
                return InvokeHttp404();

            var photo = _photoService.FindPhoto(question.PhotoId);

            var viewModel = new QuestionEditViewModel(question, photo);

            return View(viewModel);
        }

        [HttpPost("questions/{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, QuestionEditViewModel postedViewModel)
        {
            var question = _db.Questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
                return InvokeHttp404();

            if (!ModelState.IsValid)
            {
                var photo = _photoService.FindPhoto(question.PhotoId);

                var viewModel = new QuestionEditViewModel(question, photo);

                return View(viewModel);
            }

            question.Text = postedViewModel.QuestionText;
            question.SentenceStarters = postedViewModel.QuestionSentenceStarters;

            if (!String.IsNullOrWhiteSpace(postedViewModel.QuestionOrder))
            {
                int questionOrder;
                if (Int32.TryParse(postedViewModel.QuestionOrder, out questionOrder))
                    question.Order = questionOrder;
            }
            else
            {
                question.Order = null;
            }

            _db.Questions.Update(question);
            _db.SaveChanges();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("questions/{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var question = _db.Questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
                return InvokeHttp404();

            var photo = _photoService.FindPhoto(question.PhotoId);

            var viewModel = new QuestionDetailsViewModel(question, photo);

            return View(viewModel);
        }

        [HttpPost("questions/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, QuestionDetailsViewModel postedViewModel)
        {
            var question = _db.Questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
                return InvokeHttp404();

            _db.Remove(question);

            return RedirectToAction(nameof(Index));
        }
    }
}
