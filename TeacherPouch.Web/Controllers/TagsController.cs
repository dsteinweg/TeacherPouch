using System;
using System.Linq;
using System.Web.Mvc;

using TeacherPouch.Models;
using TeacherPouch.Providers;
using TeacherPouch.Repositories;
using TeacherPouch.Web.ViewModels;

namespace TeacherPouch.Web.Controllers
{
    [Authorize(Roles = TeacherPouchRoles.Admin)]
    public partial class TagsController : RepositoryControllerBase
    {
        public TagsController(IRepository repository)
        {
            base.Repository = repository;
        }


        // GET: /Tags/
        [AllowAnonymous]
        public virtual ViewResult TagIndex()
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var tags = base.Repository.GetAllTags(allowPrivate).OrderBy(tag => tag.Name);

            return View(Views.TagIndex, tags);
        }

        // GET: /Tags/spring
        [AllowAnonymous]
        public virtual ViewResult TagDetails(string tagName)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var tag = base.Repository.FindTag(tagName, allowPrivate);

            if (tag != null)
                return View(Views.TagDetails, new TagDetailsViewModel(base.Repository, tag, allowPrivate));
            else
                return InvokeHttp404();
        }

        // GET: /Tags/CreateNew
        public virtual ViewResult TagCreate()
        {
            var viewModel = new TagCreateViewModel();

            return View(Views.TagCreate, viewModel);
        }

        // POST: /Tags/CreateNew
        [HttpPost]
        public virtual ActionResult TagCreate(FormCollection collection)
        {
            var tagName = collection["Name"];
            var isPrivate = String.IsNullOrWhiteSpace(collection["IsPrivate"]) ? false : true;

            var tag = new Tag()
            {
                Name = tagName,
                IsPrivate = isPrivate
            };

            if (String.IsNullOrWhiteSpace(tagName))
            {
                var viewModel = new TagCreateViewModel();
                viewModel.Tag = tag;
                viewModel.ErrorMessage = "All fields are required.";

                return View(Views.TagCreate, viewModel);
            }
            else
            {
                base.Repository.SaveTag(tag);

                return RedirectToAction(Actions.TagDetails(tagName));
            }
        }

        // GET: /Tags/Edit/5
        public virtual ViewResult TagEdit(int id)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var tag = base.Repository.FindTag(id, allowPrivate);

            if (tag != null)
                return View(Views.TagEdit, tag);
            else
                return InvokeHttp404();
        }

        // POST: /Tags/Edit/5
        [HttpPost]
        public virtual ActionResult TagEdit(int id, FormCollection collection)
        {
            var name = collection["Name"];
            var isPrivate = String.IsNullOrWhiteSpace(collection["IsPrivate"]) ? false : true;

            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var tag = base.Repository.FindTag(id, allowPrivate);
            if (tag != null)
            {
                tag.Name = name;
                tag.IsPrivate = isPrivate;

                base.Repository.SaveTag(tag);

                return RedirectToAction(Actions.TagDetails(name));
            }
            else
            {
                return InvokeHttp404();
            }
        }

        // GET: /Tags/Delete/5
        public virtual ViewResult TagDelete(int id)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var tag = base.Repository.FindTag(id, allowPrivate);

            if (tag != null)
                return View(Views.TagDelete, tag);
            else
                return InvokeHttp404();
        }

        // POST: /Tags/Delete/5
        [HttpPost]
        public virtual ActionResult TagDelete(int id, FormCollection collection)
        {
            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var tag = base.Repository.FindTag(id, allowPrivate);

            if (tag != null)
            {
                base.Repository.DeleteTag(tag);

                return RedirectToAction(Actions.TagIndex());
            }
            else
            {
                return InvokeHttp404();
            }
        }
    }
}
