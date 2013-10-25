using System;
using System.Web.Mvc;

namespace TeacherPouch.Web.Controllers
{
    public partial class CategoryController : ControllerBase
    {
        private enum Category
        {
            CelebrationsAndHolidays,
            Clothing,
            Colors,
            Fall,
            Flowers,
            Food,
            Garden,
            Math,
            Numbers,
            Prepositions,
            School,
            Summer,
            Transportation,
            Vacation,
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
