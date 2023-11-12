using System.Net;
using System.Net.Mail;
using Business.Interfaces;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger, EmailSettings emailSettings)
    {
        _logger = logger;
        _emailSettings = emailSettings;
    }

    public async Task SendEmailConfirmation(string url, string destination)
    {
        var htmlFile = await ReadHtmlFile("../Business/Templates/Html/EmailConfirmation.html");

        htmlFile = htmlFile.Replace("{{Url}}", url);

        // Configurar o cliente SMTP
        using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
        {
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            client.EnableSsl = false;

            // Configurar a mensagem de e-mail
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SmtpFrom),
                Subject = "Assunto do E-mail",
                Body = htmlFile,
                IsBodyHtml = true
            };

            // Adicionar destinatários, CC, BCC, etc., conforme necessário
            message.To.Add(destination);

            // Enviar o e-mail
            await client.SendMailAsync(message);
        }
    }

    public async Task SendPasswordReset(string url, string destination)
    {
        var htmlFile = await ReadHtmlFile("../Business/Templates/Html/PasswordReset.html");

        htmlFile = htmlFile.Replace("{{Url}}", url);

        // Configurar o cliente SMTP
        using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
        {
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            client.EnableSsl = true;

            // Configurar a mensagem de e-mail
            var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SmtpFrom),
                Subject = "Assunto do E-mail",
                Body = htmlFile,
                IsBodyHtml = true
            };

            // Adicionar destinatários, CC, BCC, etc., conforme necessário
            message.To.Add(destination);

            // Enviar o e-mail
            await client.SendMailAsync(message);
        }
    }

    private static async Task<string> ReadHtmlFile(string path)
    {
        using var reader = new StreamReader(path);

        return await reader.ReadToEndAsync();
    }
}

public class EmailSettings
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string SmtpFrom { get; set; }
}