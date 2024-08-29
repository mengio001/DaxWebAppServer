using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.Types;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using QuizTowerPlatform.Data.DataModels;

namespace QuizTowerPlatform.Api.Accessors
{
    public interface IRequestAccessor
    {
        public IApiDbContext Db { get; }
        CurrentLoggedInUser? LoggedInUser { get; }
        CurrentLoggedInUser? AcquireCurrentUser(IIdentity? user);
        public string Authorization { get; }
    }

    // TODO: The implementation 'get CurrentLoggedInUser' is not finished yet; there is currently no option to get LoggedInUser from database.

    /// <summary>
    /// Accessor class to obtain request-scoped objects via the (injectable) RequestAccessor singleton.
    /// </summary>
    public class RequestAccessor : IRequestAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly IMemoryCache cache;

        public RequestAccessor(IHttpContextAccessor httpContextAccessor, IMemoryCache cache)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.cache = cache;
        }

        public IApiDbContext Db
        {
            get
            {
                var db = (ApiDbContext)(httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(ApiDbContext)) ?? throw new InvalidOperationException("Cannot access the ApiDbContext..."));
                db.SetCurrentUsername(LoggedInUser?.Username);
                return db;
            }
        }

        public CurrentLoggedInUser? LoggedInUser => AcquireCurrentUser(httpContextAccessor.HttpContext?.User?.Identity);

        public CurrentLoggedInUser? AcquireCurrentUser(IIdentity? user)
        {
            // TODO: note - example section to get CurrentLoggedInUser for return json data in the future.
            if (httpContextAccessor.HttpContext?.Request.RouteValues.ContainsKey("CurrentLoggedInUser") ?? false)
                return httpContextAccessor.HttpContext?.Request.RouteValues["CurrentLoggedInUser"] as CurrentLoggedInUser;

            var currentUser = new CurrentLoggedInUser
            (
                1,
                1,
                TeamType.TeamBlue
            );

            httpContextAccessor.HttpContext!.Request.RouteValues["CurrentLoggedInUser"] = currentUser;
            return currentUser;
        }

        public string Authorization => httpContextAccessor.HttpContext!.Request.Headers.Authorization.FirstOrDefault();
    }
}
