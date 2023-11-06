using Microsoft.AspNetCore.Authorization;

namespace Security.Policies;

public class AdminPolicyRequirement : IAuthorizationRequirement
{
    public AdminPolicyRequirement()
    {
    }
}