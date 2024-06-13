using Microsoft.AspNetCore.Authorization;

namespace QuizTowerPlatform.Server.Authorization;

public class MustOwnImageRequirement : IAuthorizationRequirement
{
    public MustOwnImageRequirement()
    {
    }
}
