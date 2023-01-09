using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Models;
using TeacherPouch.Services;

namespace TeacherPouch.Controllers;

public class SearchController : BaseController
{
    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    private readonly SearchService _searchService;

    [HttpGet("search")]
    public async Task<IActionResult> Search(string q, SearchOperator op = SearchOperator.Or, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return View("NoneFound");

        ViewBag.SearchTerm = q;

        if (op == SearchOperator.Or)
        {
            var results = await _searchService.SearchOr(q, cancellationToken);

            if (!results.HasAnyResults)
                return View("NoneFound");

            return View("SearchResultsOr", results);
        }
        else if (op == SearchOperator.And)
        {
            ViewBag.AndChecked = true;

            var results = await _searchService.SearchAnd(q, cancellationToken);

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
