using System;
using Microsoft.AspNetCore.Mvc;

namespace TeacherPouch.Controllers
{
    public class CategoryController : BaseController
    {
        public enum Category
        {
            Animals,
            CelebrationsAndHolidays,
            Clothing,
            Colors,
            Fall,
            Farm,
            Flowers,
            Food,
            France,
            Garden,
            Italy,
            Materials,
            Math,
            Numbers,
            Prepositions,
            School,
            Sizes,
            Summer,
            Transportation,
            Vacation,
            Weather,
            Winter,
            Zoo
        }

        [HttpGet("Category/{name}")]
        public ViewResult CategoryDetails(string name)
        {
            Category category;
            if (Enum.TryParse<Category>(name, true, out category))
            {
                return View($"~/Views/Category/{name}.cshtml");
            }
            else
            {
                return InvokeHttp404();
            }
        }
    }
}
