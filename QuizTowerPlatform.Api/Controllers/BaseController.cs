using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.Context;

namespace QuizTowerPlatform.Api.Controllers
{
    [Authorize]
    [Route(Constants.RoutePrefix + "[controller]/[action]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly IRequestAccessor requestAccessor;
        protected BaseController(IRequestAccessor requestAccessor)
        {
            this.requestAccessor = requestAccessor;
        }
        public ICurrentLoggedInUser? LoggedInUser => requestAccessor.LoggedInUser; // Note: is not finished yet, the results are dummy data but the code flow is finished.
        public IApiDbContext Db => requestAccessor.Db;
    }
}
