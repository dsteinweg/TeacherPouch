using System.Text;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Data;
using TeacherPouch.Helpers;
using TeacherPouch.ViewModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace TeacherPouch.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(
            IRepository repository,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor)
        {
            _repository = repository;
            _urlHelper = (UrlHelper)urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private readonly IRepository _repository;
        private readonly UrlHelper _urlHelper;

        [HttpGet("")]
        public ViewResult Home()
        {
            return View();
        }

        [HttpGet("About")]
        public ViewResult About()
        {
            return View();
        }

        [HttpGet("Standards")]
        public ViewResult Standards()
        {
            return View();
        }

        [HttpGet("Contact")]
        public ViewResult Contact()
        {
            return View(new ContactViewModel());
        }

        [HttpPost("Contact")]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactSubmission submision)
        {
            if (ModelState.IsValid)
            {
                // TODO: re-implement
                //if (!Request.IsLocal)
                //{
                    submision.SendEmail();
                //}
            }
            else
            {
                var viewModel = new ContactViewModel();
                viewModel.ErrorMessage = "You must fill out the form before submitting.";

                return View(viewModel);
            }

            return RedirectToAction(nameof(ContactThanks));
        }

        [HttpGet("Contact/Thanks")]
        public virtual ViewResult ContactThanks()
        {
            return View();
        }

        [HttpGet("License")]
        public virtual ViewResult License()
        {
            return View();
        }

        [HttpGet("PrivacyPolicy")]
        public virtual ViewResult PrivacyPolicy()
        {
            return View();
        }

        [HttpGet("sitemap.xml")]
        public virtual ContentResult Sitemap()
        {
            var siteMapHelper = new SiteMapHelper(_urlHelper);

            return Content(siteMapHelper.GetSiteMap(_repository, base.HttpContext), "text/xml", Encoding.UTF8);
        }
    }
}
