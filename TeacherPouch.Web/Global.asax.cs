﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using TeacherPouch.Web.Misc;
using TeacherPouch.Repositories;
using TeacherPouch.Repositories.SQLite;

namespace TeacherPouch.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ConfigureMvc();
        }

        private void ConfigureMvc()
        {
            // Change this variable instance to use a different repository.
            IRepository repositoryToUse = new SQLiteRepository();

            // Instruct the MVC runtime to use a custom controller factory to create controllers.
            // Our custom controller factory injects the model repository dependency when it instantiates controllers.
            ControllerBuilder.Current.SetControllerFactory(new CustomMvcControllerFactory(repositoryToUse));

            // Remove all view engines, including WebForm view engine. Then manually add the Razor view engine back in.
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            MvcHandler.DisableMvcResponseHeader = true;

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            base.Response.Headers.Remove("Server");
            base.Response.Headers.Remove("X-AspNet-Version");
            base.Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = base.Server.GetLastError();
            if (exception != null)
            {

            }
        }
    }
}