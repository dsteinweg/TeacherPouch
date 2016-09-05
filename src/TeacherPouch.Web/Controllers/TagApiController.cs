using Microsoft.AspNetCore.Mvc;
using TeacherPouch.Services;
using System.Collections.Generic;

namespace TeacherPouch.Controllers
{
    public class TagApiController : Controller
    {
        public TagApiController(SearchService searchService)
        {
            _searchService = searchService;
        }

        private SearchService _searchService;

        [HttpGet("api/tags")]
        public IEnumerable<string> Get(string q)
        {
            return _searchService.TagAutocompleteSearch(q);
        }
    }
}