namespace Business.Interfaces;

public interface IEmailService
{
    public Task SendEmailConfirmation(string url, string destination);
    public Task SendPasswordReset(string url, string destination);
}