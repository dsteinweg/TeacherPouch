using TeacherPouch.Web.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace TeacherPouch.Web.Controllers
{
    [Authorize]
    public partial class AdminController : ControllerBase
    {
        // GET: /Admin/
        public virtual ViewResult Index()
        {
            var viewModel = new AdminViewModel(base.User);

            return View(Views.Index, viewModel);
        }

        // GET: /Admin/Login
        [HttpGet]
        [AllowAnonymous]
        public virtual ViewResult Login()
        {
            return View(Views.Login, new LoginViewModel(null, null));
        }

        // POST: /Admin/Login
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult Login(FormCollection collection)
        {
            string userName = collection["UserName"];
            string password = collection["Password"];

            if (String.IsNullOrWhiteSpace(userName) || String.IsNullOrWhiteSpace(password))
            {
                var viewModel = new LoginViewModel(userName, "Must provide both a user name and password.");

                return View(Views.Login, viewModel);
            }

            if (Membership.ValidateUser(userName, password))
            {
                FormsAuthentication.SetAuthCookie(userName, false);
            }
            else
            {
                var viewModel = new LoginViewModel(userName, "Invalid user name and/or password.");

                return View(Views.Login, viewModel);
            }

            if (Request.Params["ReturnUrl"] != null)
                return Redirect(Request.Params["ReturnUrl"]);
            else
                return RedirectToAction(Actions.Index());
        }

        // GET: /Admin/Logout/
        public virtual ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            
            if (Request.Params["ReturnUrl"] != null)
                return Redirect(Request.Params["ReturnUrl"]);
            else
                return Redirect("/");
        }
    }
}
