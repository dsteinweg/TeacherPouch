using System;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;

namespace TeacherPouch.Controllers
{
    [Route("Category")]
    public class CategoryController : BaseController
    {
        [HttpGet("{name}")]
        public ViewResult CategoryDetails(string name)
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
