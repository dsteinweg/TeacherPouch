using System.Web.Http;

using TeacherPouch.Repositories;

namespace TeacherPouch.Web.API
{
    public abstract class ApiRepositoryControllerBase : ApiController
    {
        protected IRepository Repository { get; set; }


        public ApiRepositoryControllerBase(IRepository repository)
        {
            this.Repository = repository;
        }
    }
}