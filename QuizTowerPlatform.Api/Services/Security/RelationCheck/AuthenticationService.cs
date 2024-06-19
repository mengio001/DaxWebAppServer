using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using QuizTowerPlatform.Api.Accessors;

namespace QuizTowerPlatform.Api.Services.Security.RelationCheck
{
    public interface IAuthenticationService
    {
        public bool SetCurrentUser(IIdentity user, AuthorizationHandlerContext authContext);
    }

    public class AuthenticationService(IRequestAccessor requestAccessor, IMemoryCache cache) : IAuthenticationService
    {
        private readonly IMemoryCache cache = cache;

        // Vind de gegevens van de huidige gebruiker in de database en vul daarmee de HuidigeGebruiker-class op de RequestAccessor
        public bool SetCurrentUser(IIdentity user, AuthorizationHandlerContext authContext)
        {
            //if (requestAccessor.Client != null)
            //    return true;

            return requestAccessor.AcquireCurrentUser(user, authContext) != null;
        }
    }
}
