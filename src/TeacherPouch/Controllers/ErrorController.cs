using Microsoft.AspNetCore.Mvc;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers;

public class ErrorController : Controller
{
    [Route("Error")]
    public IActionResult Error(int? httpStatusCode, Exception? exception = null)
    {
        var showErrorDetails = User.Identity?.IsAuthenticated ?? false;

        var viewModel = new ErrorViewModel(httpStatusCode, exception, showErrorDetails);

        Response.StatusCode = viewModel.StatusCode;

        return View(viewModel);
    }
}
