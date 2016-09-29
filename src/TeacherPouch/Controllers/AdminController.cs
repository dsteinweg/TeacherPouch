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
    [Route("admin")]
    public class AdminController : BaseController
    {
        public AdminController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            var viewModel = new AdminViewModel(user, roles);

            return View(viewModel);
        }

        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel postedViewModel)
        {
            if (!ModelState.IsValid)
                return View(postedViewModel);

            var signInResult = await _signInManager.PasswordSignInAsync(
                postedViewModel.UserName,
                postedViewModel.Password,
                false,
                false);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("Login failed", "Invalid user name and/or password, " + signInResult.ToString());
                return View(postedViewModel);
            }

            StringValues values;
            if (Request.Query.TryGetValue("ReturnUrl", out values))
                return Redirect(values.First());
            else
                return RedirectToAction(nameof(Index));
        }

        [HttpGet("register")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new LoginViewModel());
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel postedViewModel)
        {
            if (!ModelState.IsValid)
                return View(postedViewModel);

            var user = new ApplicationUser
            {
                UserName = postedViewModel.UserName
            };

            var createUserResult = await _userManager.CreateAsync(user, postedViewModel.Password);

            if (!createUserResult.Succeeded)
            {
                ModelState.AddModelError("Create user failed", "Unable to create user.");
                return View(postedViewModel);
            }

            var signInResult = _signInManager.SignInAsync(user, true);

            if (!createUserResult.Succeeded)
            {
                ModelState.AddModelError("Sign in failed", "User created, but unable to sign in.");
                return View(postedViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _signInManager.SignOutAsync();

            if (!String.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);
            else
                return Redirect("/");
        }
    }
}
