using System.Text;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Home()
        {
            return View();
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet("standards")]
        public IActionResult Standards()
        {
            return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View(new ContactViewModel());
        }

        [HttpPost("contact")]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactViewModel postedViewModel)
        {
            if (ModelState.IsValid)
            {
                if (_env.IsProduction())
                {
                    //submission.SendEmail();
                }
            }
            else
            {
                postedViewModel.ErrorMessage = "You must fill out the form before submitting.";

                return View(postedViewModel);
            }

            return RedirectToAction(nameof(ContactThanks));
        }

        [HttpGet("contact/thanks")]
        public IActionResult ContactThanks()
        {
            return View();
        }

        [HttpGet("license")]
        public IActionResult License()
        {
            return View();
        }

        [HttpGet("privacy-policy")]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [HttpGet("sitemap.xml")]
        public IActionResult Sitemap()
        {
            var siteMapService = new SiteMapService(_db, _urlHelper);

            var siteMapXml = siteMapService.GenerateSiteMapXml(HttpContext);

            return Content(siteMapXml, "text/xml", Encoding.UTF8);
        }
    }
}
