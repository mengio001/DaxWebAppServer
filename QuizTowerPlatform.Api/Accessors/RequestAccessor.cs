using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.Types;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;

namespace QuizTowerPlatform.Api.Accessors
{
    public interface IRequestAccessor
    {
        public IApiDbContext Db { get; }

        public ICurrentLoggedInUser? CurrentUser { get; }

        public ICurrentLoggedInUser? AcquireCurrentUser(IIdentity? user, AuthorizationHandlerContext authContext);
    }

    /// <summary>
    /// Accessor class om request scoped objecten te verkrijgen via de (injectable) RequestAccessor singleton.
    /// </summary>
    public class RequestAccessor : IRequestAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMemoryCache cache;
        private readonly AuthorizationHandlerContext authContext;

        public RequestAccessor(IHttpContextAccessor httpContextAccessor, IMemoryCache cache, AuthorizationHandlerContext authContext)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.cache = cache;
            this.authContext = authContext;
        }

        public IApiDbContext Db
        {
            get
            {
                var db = (ApiDbContext)(httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(ApiDbContext)) ?? throw new InvalidOperationException("Kan de ApiDbContext niet te pakken krijgen..."));
                db.SetCurrentUsername(CurrentUser?.Username);
                return db;
            }
        }

        public ICurrentLoggedInUser? CurrentUser => (ICurrentLoggedInUser?)AcquireCurrentUser(httpContextAccessor.HttpContext?.User?.Identity, authContext);

        public ICurrentLoggedInUser? AcquireCurrentUser(IIdentity? user, AuthorizationHandlerContext authContext)
        {
            if (httpContextAccessor.HttpContext?.Request.RouteValues.ContainsKey("LoggedInUser") ?? false)
                return httpContextAccessor.HttpContext?.Request.RouteValues["LoggedInUser"] as CurrentLoggedInUser;

            // get the sub claim
            var ownerId = authContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            // if it cannot be found, the handler fails 
            if (ownerId == null)
            {
                authContext.Fail();
                return null;
            }

            // Get the given_name claim
            var givenName = authContext.User.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;

            // Optional: Get the custom identity name
            var customIdentity = user?.Name;

            var signInUser = cache.GetOrCreate(new { Scope = nameof(CurrentLoggedInUser), Type = nameof(AcquireCurrentUser), Filter = "oidc" }, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                    var db = (ApiDbContext)(httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(ApiDbContext)) ?? throw new InvalidOperationException("Cannot get ApiDbContext"));

                    return db.UserAccounts
                            .Where(ua => ua.SubjectId == ownerId)
                            .Select(ua => ua.User)
                            .Select(u => new
                            {
                                u!.Id,
                                Username = (u.Username.IsNullOrEmpty()? customIdentity : u.Username),
                                u.TeamId,
                                TeamType = u.Team.Type,
                            })
                            .ToList()
                            .FirstOrDefault();
                });

            if (signInUser == null)
                return null;

            var loggedInUser = new CurrentLoggedInUser
            (
                signInUser.Id,
                signInUser.TeamId,
                signInUser.TeamType.ToTeamType()
            );

            httpContextAccessor.HttpContext!.Request.RouteValues["LoggedInUser"] = loggedInUser;
            return loggedInUser;
        }
    }
}
