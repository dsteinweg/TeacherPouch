using System.Net;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers;

public class ErrorController : BaseController
{
    public IActionResult Http404()
    {
        Response.StatusCode = (int)HttpStatusCode.NotFound;

        return View("Http404");
    }

    [Route("Error")]
    public IActionResult Error(int? httpStatusCode, Exception? exception = null)
    {
        var showErrorDetails = User.Identity?.IsAuthenticated ?? false;

        var viewModel = new ErrorViewModel(httpStatusCode, exception, showErrorDetails);

        Response.StatusCode = viewModel.StatusCode;

        return View(viewModel);
    }
}
