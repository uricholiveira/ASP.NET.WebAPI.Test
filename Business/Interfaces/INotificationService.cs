using Microsoft.AspNetCore.Identity;

namespace Business.Interfaces;

public interface INotificationService
{
    public Task PublishEmailConfirmation(IdentityUser user, string emailConfirmationToken);
    public Task PublishResetPassword(IdentityUser user, string resetPasswordToken);
}