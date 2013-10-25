using System.Web.Mvc;

using TeacherPouch.Web.Helpers;
using TeacherPouch.Web.ViewModels;

namespace TeacherPouch.Web.Controllers
{
    public class ControllerBase : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            base.Response.TrySkipIisCustomErrors = true;

            var exception = filterContext.Exception;

            ErrorHelper.HandleException(exception);

            bool showErrorDetails = filterContext.HttpContext.Request.IsLocal;
            if (base.User.Identity.IsAuthenticated && base.User.Identity.Name == "darren")
                showErrorDetails = true;

            var viewModel = new ErrorViewModel(null, exception, showErrorDetails);
            var viewResult = View(MVC.Error.Views.Error, viewModel);

            filterContext.Result = viewResult;
            filterContext.ExceptionHandled = true;
        }

        public virtual ViewResult InvokeHttp404()
        {
            var errorController = new ErrorController();
            errorController.ControllerContext = base.ControllerContext;

            return errorController.Http404();
        }
    }
}
