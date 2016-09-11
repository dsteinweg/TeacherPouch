using System;
using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Services;

namespace TeacherPouch.Controllers
{
    public class SearchController : BaseController
    {
        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        private readonly SearchService _searchService;

        [HttpGet("search")]
        public IActionResult Search(string q, SearchOperator op = SearchOperator.Or)
        {
            if (String.IsNullOrWhiteSpace(q) || q.Length < 2)
                return View("NoneFound");

            ViewBag.SearchTerm = q;

            if (op == SearchOperator.Or)
            {
                var results = _searchService.SearchOr(q);

                if (!results.HasAnyResults)
                    return View("NoneFound");

                return View("SearchResultsOr", results);
            }
            else if (op == SearchOperator.And)
            {
                ViewBag.AndChecked = true;

                var results = _searchService.SearchAnd(q);

                if (!results.HasAnyResults)
                    return View("NoneFound");

                return View("SearchResultsAnd", results);
            }
            else
            {
                throw new Exception("Unknown search operator.");
            }
        }
    }
}
