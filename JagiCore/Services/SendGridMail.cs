using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JagiCore.Services
{
    public class SendGridMail : IEmailSender
    {
        private readonly EmailSetting _settings;

        public SendGridMail(EmailSetting settings)
        {
            _settings = settings;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net");
            smtpClient.Credentials = new System.Net.NetworkCredential(_settings.Email, _settings.Password);
            smtpClient.EnableSsl = true;
            smtpClient.Port = 587;

            mail.From = new MailAddress(_settings.ReturnEmail, _settings.Title);
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;

            await smtpClient.SendMailAsync(mail);
        }
    }
}
