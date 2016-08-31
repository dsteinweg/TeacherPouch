using Microsoft.AspNetCore.Mvc;

namespace TeacherPouch.Controllers
{
    public class BaseController : Controller
    {
        public ViewResult InvokeHttp404()
        {
            var errorController = new ErrorController();
            errorController.ControllerContext = ControllerContext;

            return errorController.Http404();
        }
    }
}
