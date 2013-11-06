﻿using System.Text;
using System.Web.Mvc;

using TeacherPouch.Models;
using TeacherPouch.Repositories;
using TeacherPouch.Web.Helpers;
using TeacherPouch.Web.ViewModels;

namespace TeacherPouch.Web.Controllers
{
    public partial class PagesController : RepositoryControllerBase
    {
        public PagesController(IRepository repository)
        {
            base.Repository = repository;
        }


        // GET: /
        public virtual ViewResult Home()
        {
            return View(Views.Home);
        }

        public virtual ViewResult About()
        {
            return View(Views.About);
        }

        // GET: /Contact
        public virtual ViewResult Contact()
        {
            var viewModel = new ContactViewModel();

            return View(Views.Contact, viewModel);
        }

        // POST: /Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Contact(ContactSubmission submision)
        {
            if (submision.IsValid)
            {
                if (!base.Request.IsLocal)
                {
                    submision.SendEmail();
                }
            }
            else
            {
                var viewModel = new ContactViewModel();
                viewModel.ErrorMessage = "You must fill out the form before submitting.";

                return View(Views.Contact, viewModel);
            }

            return RedirectToAction(Actions.ContactThanks());
        }

        // GET: /Contact/Thanks
        public virtual ViewResult ContactThanks()
        {
            return View(Views.ContactThanks);
        }

        // GET: /License
        public virtual ViewResult License()
        {
            return View(Views.License);
        }

        // GET: /sitemap.xml
        public virtual ContentResult Sitemap()
        {
            return Content(SiteMapHelper.GetSiteMap(base.Repository, base.HttpContext), "text/xml", Encoding.UTF8);
        }
    }
}
