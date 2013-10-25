using System;
using System.Web.Mvc;

using TeacherPouch.Models;
using TeacherPouch.Web.ViewModels;

namespace TeacherPouch.Web.Controllers
{
    public partial class PagesController : ControllerBase
    {
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

        // GET: /Copyright
        public virtual ViewResult Copyright()
        {
            return View(Views.Copyright);
        }
    }
}
