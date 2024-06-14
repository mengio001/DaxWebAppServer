using Microsoft.AspNetCore.Authorization;

namespace QuizTowerPlatform.API.Authorization
{
    public class MustOwnImageAttribute : AuthorizeAttribute, IAuthorizationRequirementData
    {
        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            return new[] { new MustOwnImageRequirement() };
        }
    }
}