using TeacherPouch.Repositories;

namespace TeacherPouch.Web.Controllers
{
    public class RepositoryControllerBase : ControllerBase
    {
        protected IRepository Repository { get; set; }
    }
}
