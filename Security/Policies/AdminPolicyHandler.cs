using Microsoft.AspNetCore.Authorization;

namespace Security.Policies;

public class AdminPolicyHandler: AuthorizationHandler<AdminPolicyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminPolicyRequirement requirement)
    {
        var user = context.User;

        return Task.CompletedTask;
    }
}