using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Helper.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ExtensioProcuratio.Helper
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IConfiguration configuration, IOptions<SmtpSettings> settings)
        {
            _configuration = configuration;
            _smtpSettings = settings.Value;
        }

        public async Task<bool> SendEmailAsync(EmailModel email)
        {
            try
            {
                var mail = new MailMessage()
                {
                    From = new MailAddress(_smtpSettings.UserName, _smtpSettings.Name),
                    Subject = email.Subject,
                    Body = email.Body,
                    IsBodyHtml = true,
                    Priority = MailPriority.High
                };

                mail.To.Add(email.Email);

                using (var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Door))
                {
                    smtp.Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Pass);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}