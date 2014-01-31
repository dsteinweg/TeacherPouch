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

            ViewBag.SearchTerm = q;

            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            SearchOperator searchOperator = SearchOperator.Or;
            if (!String.IsNullOrWhiteSpace(op))
            {
                SearchOperator parsed;
                if (Enum.TryParse(op, true, out parsed))
                    searchOperator = parsed;
            }

            if (searchOperator == SearchOperator.Or)
            {
                var results = base.Repository.SearchOr(q, allowPrivate);

                if (results.HasAnyResults)
                    return base.View(Views.SearchResultsOr, results);
                else
                    return base.View(Views.NoneFound);
            }
            else if (searchOperator == SearchOperator.And)
            {
                ViewBag.AndChecked = true;

                var results = base.Repository.SearchAnd(q, allowPrivate);

                if (results.HasAnyResults)
                    return base.View(Views.SearchResultsAnd, results);
                else
                    return base.View(Views.NoneFound);
            }
            else
            {
                throw new ApplicationException("Search operator not found.");
            }
        }
    }
}
