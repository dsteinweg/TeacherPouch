using System;
using System.ComponentModel;
using System.Web.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Providers;
using TeacherPouch.Repositories;

namespace TeacherPouch.Web.Controllers
{
    public partial class SearchController : RepositoryControllerBase
    {
        public SearchController(IRepository repository)
        {
            base.Repository = repository;
        }


        // GET: /Search?q=spring&op=and
        public virtual ViewResult Search(string q, SearchOperator op = SearchOperator.Or)
        {
            if (String.IsNullOrWhiteSpace(q) || q.Length < 2)
                return base.View(Views.NoneFound);

            ViewBag.SearchTerm = q;

            bool allowPrivate = SecurityHelper.UserCanSeePrivateRecords(base.User);

            if (op == SearchOperator.Or)
            {
                var results = base.Repository.SearchOr(q, allowPrivate);

                if (results.HasAnyResults)
                    return base.View(Views.SearchResultsOr, results);
                else
                    return base.View(Views.NoneFound);
            }
            else if (op == SearchOperator.And)
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
                throw new InvalidEnumArgumentException("Unknown search operator.");
            }
        }
    }
}
