using Microsoft.AspNetCore.Authorization;

namespace QuizTowerPlatform.API.Authorization;

public class MustOwnImageRequirement : IAuthorizationRequirement
{
    public MustOwnImageRequirement()
    {
    }
}