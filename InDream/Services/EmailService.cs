using InDream.Common.Configuration;
using InDream.Core.DI;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace InDream.Services;

public class EmailService : IInjectAsScoped
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }


    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mail = new MailMessage();
        mail.To.Add(new MailAddress(toEmail));
        mail.From = new MailAddress(_settings.FromEmail);
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;

        using (var smtp = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort))
        {
            smtp.Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password);
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(mail);
        }
    }
}
