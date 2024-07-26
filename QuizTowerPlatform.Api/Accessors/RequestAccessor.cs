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
    }

    /// <summary>
    /// Accessor class to obtain request-scoped objects via the (injectable) RequestAccessor singleton.
    /// </summary>
    public class RequestAccessor : IRequestAccessor
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public RequestAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IApiDbContext Db
        {
            get
            {
                var db = (ApiDbContext)(httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(ApiDbContext)) ?? throw new InvalidOperationException("Cannot access the ApiDbContext..."));
                return db;
            }
        }
    }
}
