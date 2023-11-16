namespace Business.Interfaces;

public interface IEmailService
{
    public Task SendEmailConfirmation(string name, string url, string destination);
    public Task SendPasswordReset(string name, string url, string destination);
}