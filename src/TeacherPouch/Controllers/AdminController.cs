using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.ViewModels;

namespace TeacherPouch.Controllers;

[Authorize]
[Route("admin")]
public class AdminController : Controller
{
    public AdminController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var adminRole = new IdentityRole(TeacherPouchRoles.Admin);
        if (!await _roleManager.RoleExistsAsync(adminRole.Name!))
            _ = await _roleManager.CreateAsync(adminRole);

        var friendRole = new IdentityRole(TeacherPouchRoles.Friend);
        if (!await _roleManager.RoleExistsAsync(friendRole.Name!))
            _ = await _roleManager.CreateAsync(friendRole);

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return NotFound();

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
            postedViewModel.UserName!,
            postedViewModel.Password!,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError("Login failed", "Invalid user name and/or password, " + signInResult.ToString());
            return View(postedViewModel);
        }

        if (Request.Query.TryGetValue("ReturnUrl", out var values))
            return Redirect(values.First()!);
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

        var user = new IdentityUser { UserName = postedViewModel.UserName };

        var createUserResult = await _userManager.CreateAsync(user, postedViewModel.Password!);

        if (!createUserResult.Succeeded)
        {
            ModelState.AddModelError("Create user failed", "Unable to create user.");
            return View(postedViewModel);
        }

        var signInResult = _signInManager.SignInAsync(user, isPersistent: true);

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

        if (!string.IsNullOrWhiteSpace(returnUrl))
            return Redirect(returnUrl);
        else
            return Redirect("/");
    }
}
