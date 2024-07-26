using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.Context;

namespace QuizTowerPlatform.Api.Controllers
{
    [Authorize]
    [Route(Constants.RoutePrefix + "[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        private readonly IRequestAccessor requestAccessor;
        protected BaseController(IRequestAccessor requestAccessor)
        {
            this.requestAccessor = requestAccessor;
        }
        public IApiDbContext Db => requestAccessor.Db;
    }
}
