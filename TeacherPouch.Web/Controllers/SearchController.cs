using System;
using System.Web.Mvc;

using TeacherPouch.Repositories;
using TeacherPouch.Utilities.Extensions;
using TeacherPouch.Providers;
using TeacherPouch.Models;

namespace TeacherPouch.Web.Controllers
{
    public partial class SearchController : RepositoryControllerBase
    {
        public SearchController(IRepository repository)
        {
            base.Repository = repository;
        }


        // GET: /Search?q=spring&op=and
        public virtual ViewResult Search(string q, string op)
        {
            if (String.IsNullOrWhiteSpace(q) || q.Length <= 2)
                return base.View(Views.NoneFound);

            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            SearchOperator searchOperator = SearchOperator.Or;
            if (!String.IsNullOrWhiteSpace(op))
            {
                SearchOperator parsed;
                if (Enum.TryParse(op, true, out parsed))
                    searchOperator = parsed;
            }

            var results = base.Repository.Search(q, searchOperator, allowPrivate);

            if (results.HasAnyResults)
                return base.View(Views.SearchResults, results);
            else
                return base.View(Views.NoneFound);
        }
    }
}
