using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using QuizTowerPlatform.Api.Accessors;

namespace QuizTowerPlatform.Api.Services.Security.RelationCheck
{
    //public interface IAuthenticationService
    //{
    //    public bool SetCurrentUser(IIdentity user, AuthorizationHandlerContext authContext);
    //}

    //public class AuthenticationService(IRequestAccessor requestAccessor, IMemoryCache cache) : IAuthenticationService
    //{
    //    private readonly IMemoryCache cache = cache;

    //
    //    // Find the current user's data in the database and populate the HuidigeGebruiker class on the RequestAccessor.
    //    public bool SetCurrentUser(IIdentity user, AuthorizationHandlerContext authContext)
    //    {
    //        //if (requestAccessor.Client != null)
    //        //    return true;

    //        return requestAccessor.AcquireCurrentUser(user, authContext) != null;
    //    }
    //}
}
