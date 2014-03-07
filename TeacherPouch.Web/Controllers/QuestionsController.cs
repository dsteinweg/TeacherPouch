using System;
using System.Web.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Providers;
using TeacherPouch.Repositories;
using TeacherPouch.Web.ViewModels;

namespace TeacherPouch.Web.Controllers
{
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public partial class QuestionsController : RepositoryControllerBase
    {
        public QuestionsController(IRepository repository)
        {
            base.Repository = repository;
        }


        // GET: /Questions
        [AllowAnonymous]
        public virtual ViewResult QuestionIndex()
        {
            var viewModel = new QuestionIndexViewModel();
            viewModel.Questions = this.Repository.GetAllQuestions();
            viewModel.DisplayAdminLinks = SecurityHelper.UserIsAdmin(base.User);

            return View(Views.QuestionIndex, viewModel);
        }

        // GET: /Questions/{id}
        [AllowAnonymous]
        public virtual ViewResult QuestionDetails(int id)
        {
            var question = this.Repository.FindQuestion(id);
            if (question == null)
                return InvokeHttp404();

            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);
            var photo = this.Repository.FindPhoto(question.PhotoID, allowPrivate);
            var photoTags = this.Repository.GetTagsForPhoto(photo, allowPrivate);

            var viewModel = new QuestionDetailsViewModel(question, photo, photoTags);
            viewModel.DisplayAdminLinks = SecurityHelper.UserIsAdmin(base.User);

            return View(Views.QuestionDetails, viewModel);
        }

        // GET: /Photos/{photoID}/Questions/Create
        [HttpGet]
        public virtual ViewResult QuestionCreate(int photoID)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var photo = this.Repository.FindPhoto(photoID, allowPrivate);
            if (photo == null)
                return InvokeHttp404();

            var photoTags = this.Repository.GetTagsForPhoto(photo, allowPrivate);

            var viewModel = new QuestionCreateViewModel(photo, photoTags);

            return View(Views.QuestionCreate, viewModel);
        }

        // POST: /Photos/{photoID}/Questions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult QuestionCreate(QuestionCreateViewModel postedViewModel)
        {
            if (ModelState.IsValid)
            {
                var question = new Question();
                question.PhotoID = postedViewModel.Photo.ID;
                question.Text = postedViewModel.QuestionText;
                question.SentenceStarters = postedViewModel.QuestionSentenceStarters;

                int questionOrder;
                if (Int32.TryParse(postedViewModel.QuestionOrder, out questionOrder))
                    question.Order = questionOrder;

                this.Repository.InsertQuestion(question);

                return RedirectToAction(MVC.Questions.QuestionDetails(question.ID));
            }
            else
            {
                bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

                var photo = this.Repository.FindPhoto(postedViewModel.Photo.ID, allowPrivate);
                if (photo == null)
                    return InvokeHttp404();

                var photoTags = this.Repository.GetTagsForPhoto(photo, allowPrivate);

                var viewModel = new QuestionCreateViewModel(photo, photoTags);

                return View(Views.QuestionCreate, viewModel);
            }
        }

        // GET: /Questions/{id}/Edit
        public virtual ViewResult QuestionEdit(int id)
        {
            var question = this.Repository.FindQuestion(id);
            if (question == null)
                return InvokeHttp404();

            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);
            var photo = this.Repository.FindPhoto(question.PhotoID, allowPrivate);
            var photoTags = this.Repository.GetTagsForPhoto(photo, allowPrivate);

            var viewModel = new QuestionEditViewModel(question, photo, photoTags);

            return View(Views.QuestionEdit, viewModel);
        }

        // POST: /Questions/{id}/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult QuestionEdit(QuestionEditViewModel postedViewModel)
        {
            var question = this.Repository.FindQuestion(postedViewModel.QuestionID);
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

                this.Repository.UpdateQuestion(question);

                return RedirectToAction(MVC.Questions.QuestionDetails(question.ID));
            }
            else
            {
                var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);
                var photo = this.Repository.FindPhoto(question.PhotoID, allowPrivate);
                var photoTags = this.Repository.GetTagsForPhoto(photo, allowPrivate);

                var viewModel = new QuestionEditViewModel(question, photo, photoTags);

                return View(Views.QuestionEdit, viewModel);
            }
        }

        // GET: /Questions/{id}/Delete
        public virtual ViewResult QuestionDelete(int id)
        {
            var question = this.Repository.FindQuestion(id);
            if (question == null)
                return InvokeHttp404();

            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);
            var photo = this.Repository.FindPhoto(question.PhotoID, allowPrivate);
            var photoTags = this.Repository.GetTagsForPhoto(photo, allowPrivate);

            var viewModel = new QuestionDetailsViewModel(question, photo, photoTags);

            return View(Views.QuestionDelete, viewModel);
        }

        // POST: /Questions/{id}/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult QuestionDelete(QuestionDetailsViewModel postedViewModel)
        {
            var question = this.Repository.FindQuestion(postedViewModel.QuestionID);
            if (question == null)
                return InvokeHttp404();

            this.Repository.DeleteQuestion(question);

            return RedirectToAction(MVC.Questions.QuestionIndex());
        }

        // GET: /Photos/{photoID}/Questions
        [AllowAnonymous]
        public virtual ViewResult QuestionsForPhoto(int photoID)
        {
            return View(Views.QuestionsForPhoto);
        }
    }
}
