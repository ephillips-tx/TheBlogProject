using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using TheBlogProject.ViewModels;

namespace TheBlogProject.Services
{
    public class EmailService : IBlogEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            // Provides access to mail settings in appsettings.json
            _mailSettings = mailSettings.Value;
        }

        // Send from contact form
        public async Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(_mailSettings.Mail));
            email.Subject = subject;

            var bBuilder = new BodyBuilder()
            {
                HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}"
            };

            email.Body = bBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);

        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;

            //var bBuilder = new BodyBuilder();
            //bBuilder.HtmlBody = htmlMessage;
            var bBuilder = new BodyBuilder()
            {
                HtmlBody = htmlMessage
            };

            email.Body = bBuilder.ToMessageBody();

            using var smtp = new SmtpClient(); // used to leverage gmail smtp server
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls); // Uses transport layer security with SecureSocketOptions
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);


            await smtp.SendAsync(email);

            smtp.Disconnect(true);

            // Could add code for adding attachments here

        }
    }
}
