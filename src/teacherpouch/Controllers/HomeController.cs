using System.Text;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Data;
using TeacherPouch.Helpers;
using TeacherPouch.ViewModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;

namespace TeacherPouch.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(
            TeacherPouchDbContext dbContext,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IHostingEnvironment env)
        {
            _db = dbContext;
            _urlHelper = (UrlHelper)urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _env = env;
        }

        private readonly TeacherPouchDbContext _db;
        private readonly UrlHelper _urlHelper;
        private readonly IHostingEnvironment _env;

        [HttpGet("")]
        public ViewResult Home()
        {
            return View();
        }

        [HttpGet("about")]
        public ViewResult About()
        {
            return View();
        }

        [HttpGet("standards")]
        public ViewResult Standards()
        {
            return View();
        }

        [HttpGet("contact")]
        public ViewResult Contact()
        {
            return View(new ContactViewModel());
        }

        [HttpPost("contact")]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactSubmission submission)
        {
            if (ModelState.IsValid)
            {
                if (_env.IsProduction())
                {
                    submission.SendEmail();
                }
            }
            else
            {
                var viewModel = new ContactViewModel
                {
                    ErrorMessage = "You must fill out the form before submitting."
                };

                return View(viewModel);
            }

            return RedirectToAction(nameof(ContactThanks));
        }

        [HttpGet("contact/thanks")]
        public virtual ViewResult ContactThanks()
        {
            return View();
        }

        [HttpGet("license")]
        public virtual ViewResult License()
        {
            return View();
        }

        [HttpGet("privacy-policy")]
        public virtual ViewResult PrivacyPolicy()
        {
            return View();
        }

        [HttpGet("sitemap.xml")]
        public virtual ContentResult Sitemap()
        {
            var siteMapService = new SiteMapService(_db, _urlHelper);

            var siteMapXml = siteMapService.GenerateSiteMapXml(HttpContext);

            return Content(siteMapXml, "text/xml", Encoding.UTF8);
        }
    }
}