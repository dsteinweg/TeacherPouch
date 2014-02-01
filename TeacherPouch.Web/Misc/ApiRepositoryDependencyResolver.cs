using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using TeacherPouch.Repositories;
using TeacherPouch.Web.API;

namespace TeacherPouch.Web.Misc
{
    public class ApiRepositoryDependencyResolver : IDependencyResolver
    {
        private IRepository Repository = null;


        public ApiRepositoryDependencyResolver(IRepository repository)
        {
            this.Repository = repository;
        }


        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType.IsSubclassOf(typeof(ApiRepositoryControllerBase)))
                return Activator.CreateInstance(serviceType, this.Repository);
            else
                return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public void Dispose()
        {
            
        }
    }
}