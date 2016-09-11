using System;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;

namespace TeacherPouch.Controllers
{
    [Route("category")]
    public class CategoryController : BaseController
    {
        [HttpGet("{name}")]
        public IActionResult CategoryDetails(string name)
        {
            Category category;
            if (Enum.TryParse<Category>(name, true, out category))
            {
                return View($"~/Views/Category/{category}.cshtml");
            }
            else
            {
                return InvokeHttp404();
            }
        }
    }
}
