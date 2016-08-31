using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TeacherPouch.Models;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        [HttpGet("Admin")]
        public async Task<ViewResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            var viewModel = new AdminViewModel(user, roles);

            return View(viewModel);
        }

        [HttpGet("Admin/Login")]
        [AllowAnonymous]
        public ViewResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost("Admin/Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (String.IsNullOrWhiteSpace(loginViewModel.UserName) || String.IsNullOrWhiteSpace(loginViewModel.Password))
            {
                loginViewModel.LoginErrorMessage = "Must provide both a user name and password.";

                return View(loginViewModel);
            }

            var user = new ApplicationUser
            {
                UserName = loginViewModel.UserName,
                Password = loginViewModel.Password
            };

            var userValidator = new UserValidator<ApplicationUser>();
            var result = await userValidator.ValidateAsync(_userManager, user);

            var signInResult = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);

            if (!signInResult.Succeeded)
            {
                loginViewModel.LoginErrorMessage = "Invalid user name and/or password.";

                return View(loginViewModel);
            }

            StringValues values;
            if (Request.Query.TryGetValue("ReturnUrl", out values))
                return Redirect(values.First());
            else
                return RedirectToAction(nameof(Index));
        }

        [HttpGet("Admin/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            
            StringValues values;
            if (Request.Query.TryGetValue("ReturnUrl", out values))
                return Redirect(values.First());
            else
                return Redirect("/");
        }
    }
}
