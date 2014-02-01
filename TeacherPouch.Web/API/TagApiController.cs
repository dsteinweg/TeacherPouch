using System.Collections.Generic;
using System.Web.Http;

using TeacherPouch.Providers;
using TeacherPouch.Repositories;

namespace TeacherPouch.Web.API
{
    public class TagApiController : ApiRepositoryControllerBase
    {
        public TagApiController(IRepository repository)
            : base(repository)
        {

        }


        [HttpGet]
        [Route("api/tags")]
        public IEnumerable<string> Get([FromUri] string q)
        {
            var allowPrivate = SecurityHelper.UserCanSeePrivateRecords(User);

            return base.Repository.TagAutocompleteSearch(q, allowPrivate);
        }
    }
}