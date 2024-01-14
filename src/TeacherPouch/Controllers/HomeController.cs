using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Data;
using TeacherPouch.Services;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers;

public class HomeController(TeacherPouchDbContext _dbContext, IWebHostEnvironment _env) : Controller
{
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
        var siteMapService = new SiteMapService(_dbContext, Url);

        var siteMapXml = siteMapService.GenerateSiteMapXml(HttpContext);

        return Content(siteMapXml, MediaTypeNames.Text.Xml, Encoding.UTF8);
    }
}
