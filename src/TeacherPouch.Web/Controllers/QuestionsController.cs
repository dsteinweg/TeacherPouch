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
    public partial class QuestionsController : BaseController
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

        [HttpGet("Questions")]
        [AllowAnonymous]
        public ViewResult QuestionIndex()
        {
            var viewModel = new QuestionIndexViewModel();
            viewModel.Questions = _db.Questions.ToList();
            viewModel.DisplayAdminLinks = User.IsInRole(TeacherPouchRoles.Admin);

            return View(viewModel);
        }

        [HttpGet("Questions/{id:int}")]
        [AllowAnonymous]
        public ViewResult QuestionDetails(int id)
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

        [HttpGet("Photos/{photoId:int}/Questions")]
        [AllowAnonymous]
        public ViewResult QuestionsForPhoto(int photoId)
        {
            return View();
        }

        [HttpGet("Photos/{photoId:int}/Questions/Create")]
        public ViewResult QuestionCreate(int photoId)
        {
            var photo = _photoService.FindPhoto(photoId);
            if (photo == null)
                return InvokeHttp404();

            var viewModel = new QuestionCreateViewModel(photo);

            return View(viewModel);
        }

        [HttpPost("Photos/{photoId:int}/Questions/Create")]
        [ValidateAntiForgeryToken]
        public ActionResult QuestionCreate(int photoId, QuestionCreateViewModel postedViewModel)
        {
            if (ModelState.IsValid)
            {
                var question = new Question
                {
                    PhotoId = photoId,
                    Text = postedViewModel.QuestionText,
                    SentenceStarters = postedViewModel.QuestionSentenceStarters
                };

                int questionOrder;
                if (Int32.TryParse(postedViewModel.QuestionOrder, out questionOrder))
                    question.Order = questionOrder;

                _db.Questions.Add(question);

                _db.SaveChanges();

                return RedirectToAction(nameof(QuestionDetails), new { id = question.Id });
            }
            else
            {
                var photo = _photoService.FindPhoto(photoId);
                if (photo == null)
                    return InvokeHttp404();

                var viewModel = new QuestionCreateViewModel(photo);

                return View(viewModel);
            }
        }

        [HttpGet("Questions/{id:int}/Edit")]
        public ViewResult QuestionEdit(int id)
        {
            var question = _db.Questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
                return InvokeHttp404();

            var photo = _photoService.FindPhoto(question.PhotoId);

            var viewModel = new QuestionEditViewModel(question, photo);

            return View(viewModel);
        }

        [HttpPost("Questions/{id:int}/Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult QuestionEdit(int id, QuestionEditViewModel postedViewModel)
        {
            var question = _db.Questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
                return InvokeHttp404();

            if (ModelState.IsValid)
            {
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

                return RedirectToAction(nameof(QuestionDetails), new { id });
            }
            else
            {
                var photo = _photoService.FindPhoto(question.PhotoId);

                var viewModel = new QuestionEditViewModel(question, photo);

                return View(viewModel);
            }
        }

        [HttpGet("Questions/{id:int}/Delete")]
        public ViewResult QuestionDelete(int id)
        {
            var question = _db.Questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
                return InvokeHttp404();

            var photo = _photoService.FindPhoto(question.PhotoId);

            var viewModel = new QuestionDetailsViewModel(question, photo);

            return View(viewModel);
        }

        [HttpPost("Questoins/{id:int}/Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult QuestionDelete(int id, QuestionDetailsViewModel postedViewModel)
        {
            var question = _db.Questions.FirstOrDefault(q => q.Id == id);
            if (question == null)
                return InvokeHttp404();

            _db.Remove(question);

            return RedirectToAction(nameof(QuestionIndex));
        }
    }
}
