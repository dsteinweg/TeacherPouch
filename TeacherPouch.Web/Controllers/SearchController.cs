using System;
using System.Web.Mvc;

using TeacherPouch.Repositories;
using TeacherPouch.Utilities.Extensions;
using TeacherPouch.Providers;

namespace TeacherPouch.Web.Controllers
{
    public partial class SearchController : RepositoryControllerBase
    {
        public SearchController(IRepository repository)
        {
            base.Repository = repository;
        }


        // GET: /Search?q=spring
        public virtual ViewResult Search()
        {
            var query = Request.QueryString["q"];

            if (String.IsNullOrWhiteSpace(query) || query.Length <= 2)
                return base.View(Views.NoneFound);

            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            var results = base.Repository.Search(query, allowPrivate);

            if (results.HasAnyResults)
                return base.View(Views.SearchResults, results);
            else
                return base.View(Views.NoneFound);
        }
    }
}
