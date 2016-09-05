using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers
{
    public class ErrorController : BaseController
    {
        public ViewResult Http404()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View();
        }

        [Route("Error")]
        public ViewResult Error(int? httpStatusCode, Exception exception = null)
        {
            var showErrorDetails = User.Identity.IsAuthenticated;

            var viewModel = new ErrorViewModel(httpStatusCode, exception, showErrorDetails);

            Response.StatusCode = viewModel.StatusCode;

            return View(viewModel);
        }
    }
}
