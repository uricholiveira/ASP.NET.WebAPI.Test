using Microsoft.AspNetCore.Identity;

namespace Business.Interfaces;

public interface INotificationService
{
    public Task SendEmailConfirmation(IdentityUser user, string emailConfirmationToken);
    public Task SendPasswordReset(IdentityUser user, string passwordConfirmationToken);
}