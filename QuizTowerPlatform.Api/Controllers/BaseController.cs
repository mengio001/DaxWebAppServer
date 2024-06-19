using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.Context;

namespace QuizTowerPlatform.Api.Controllers
{
    [Authorize]
    [Route(Constants.ControllerPathPrefix + "/[controller]")]
    //[Produces("application/json")]
    //[Consumes("application/json")]
    public abstract class BaseController : ControllerBase
    {
        private readonly IRequestAccessor requestAccessor;
        protected BaseController(IRequestAccessor requestAccessor)
        {
            this.requestAccessor = requestAccessor;
        }

        public ICurrentLoggedInUser CurrentUser => requestAccessor.CurrentUser;
        public IApiDbContext Db => requestAccessor.Db;
    }
}
