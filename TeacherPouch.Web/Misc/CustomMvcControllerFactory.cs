using System;
using System.Web.Mvc;
using System.Web.Routing;

using TeacherPouch.Web.Controllers;
using TeacherPouch.Repositories;

namespace TeacherPouch.Web.Misc
{
    public class CustomMvcControllerFactory : DefaultControllerFactory
    {
        private readonly IRepository Repository;

        public CustomMvcControllerFactory(IRepository repository)
        {
            this.Repository = repository;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType.IsSubclassOf(typeof(RepositoryControllerBase)))
                return Activator.CreateInstance(controllerType, Repository) as IController;
            else
                return Activator.CreateInstance(controllerType) as IController;
        }
    }
}
