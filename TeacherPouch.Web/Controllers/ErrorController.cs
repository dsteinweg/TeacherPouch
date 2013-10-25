using System;
using System.Net;
using System.Web.Mvc;

using TeacherPouch.Web.Helpers;
using TeacherPouch.Web.ViewModels;

namespace TeacherPouch.Web.Controllers
{
    public partial class ErrorController : ControllerBase
    {
        public virtual ViewResult Http404()
        {
            var relativeUrl = base.ControllerContext.RequestContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath;

            ErrorHelper.HandleError(relativeUrl);

            base.ControllerContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View(Views.NotFound);
        }

        public virtual ViewResult Error(int? httpStatusCode, Exception exception = null)
        {
            var request = base.ControllerContext.RequestContext.HttpContext.Request;

            var relativeUrl = request.AppRelativeCurrentExecutionFilePath;
            ErrorHelper.HandleError(relativeUrl);

            bool showErrorDetails = request.IsLocal;
            if (base.User.Identity.IsAuthenticated && base.User.Identity.Name == "darren")
                showErrorDetails = true;

            var viewModel = new ErrorViewModel(httpStatusCode, exception, showErrorDetails);

            base.ControllerContext.HttpContext.Response.StatusCode = viewModel.StatusCode;

            return View(Views.Error, viewModel);
        }
    }
}
