using System;
using System.Web.Mvc;

namespace TeacherPouch.Web.Controllers
{
    public partial class CategoryController : ControllerBase
    {
        public enum Category
        {
            CelebrationsAndHolidays,
            Clothing,
            Colors,
            Fall,
            Farm,
            Flowers,
            Food,
            Garden,
            Italy,
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

        // GET: /Category/Food
        public virtual ViewResult CategoryDetails(string name)
        {
            Category category;
            if (Enum.TryParse<Category>(name, true, out category))
            {
                string viewName = String.Format("~/Views/Category/{0}.cshtml", name);

                return View(viewName);
            }
            else
            {
                return InvokeHttp404();
            }
        }
    }
}
