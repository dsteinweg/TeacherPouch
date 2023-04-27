using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;

namespace TeacherPouch.Controllers;

[Route("category")]
public class CategoryController : Controller
{
    [HttpGet("{name}")]
    public IActionResult CategoryDetails(string name)
    {
        if (Enum.TryParse<Category>(name, true, out var category))
            return View($"~/Views/Category/{category}.cshtml");
        else
            return NotFound();
    }
}
