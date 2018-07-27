using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JagiCore.Services
{
    public class EmailSetting
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReturnEmail { get; set; }
        public string Title { get; set; }
    }

    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class GEmailSender : IEmailSender
    {
        private readonly Mailer _mailer;

        public GEmailSender(Mailer mailer)
        {
            _mailer = mailer;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            _mailer.SendGMail(email, message, subject).Wait();

            return Task.CompletedTask;
        }
    }

    public class Mailer
    {
        private EmailSetting _settings;

        public Mailer(EmailSetting settings)
        {
            this._settings = settings;
            CompleteEmptyField();
        }

        /// <summary>
        /// 僅提供 GMail 的發送 EMAIL，須先設定 EmailSetting (使用 constructor)
        /// 若有需要知道是否完成發送，可以使用 callback & eventState
        /// </summary>
        /// <param name="emailAddress">需要發送 email 的 address</param>
        /// <param name="content">EMAIL 內容</param>
        /// <param name="subject">EMAIL 標題</param>
        /// <param name="callback">Call back，標準做法：new WaitCallback((o) => { // do something you want ((AutoResetEvent)o).Set(); }); </param>
        /// <param name="autoEvent">讓客戶端知道何時完成，標準做法： AutoResetEvent autoEvent = new AutoResetEvent(false);</param>
        public async Task SendGMail(string emailAddress, string content, string subject,
            WaitCallback callback = null, AutoResetEvent eventState = null)
        {
            MailMessage mail = new MailMessage();
            //using ()
            //{

            //}
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Credentials = new System.Net.NetworkCredential(_settings.Email, _settings.Password);
            smtpClient.EnableSsl = true;
            smtpClient.Port = 587;

            mail.From = new MailAddress(_settings.ReturnEmail, _settings.Title, System.Text.Encoding.Default);
            mail.To.Add(emailAddress);
            mail.Subject = subject;
            mail.Body = content;
            mail.IsBodyHtml = true;

            string tail = "<br />\n---------------------------\n<br />";
            tail += "本封信件為系統自動發送，請勿直接回信；若有問題請與系統管理員聯繫。";

            mail.Body += tail;

            if (callback != null)
            {
                smtpClient.SendCompleted += (s, e) => {
                    object obj = null;
                    if (eventState != null)
                        obj = eventState;
                    callback(obj);
                };
            }

            await smtpClient.SendMailAsync(mail);
        }

        private void CompleteEmptyField()
        {
            if (string.IsNullOrEmpty(_settings.Email))
                throw new ArgumentException("Email 設定不可為空值");

            if (string.IsNullOrEmpty(this._settings.ReturnEmail))
                this._settings.ReturnEmail = this._settings.Email;
            if (string.IsNullOrEmpty(this._settings.Title))
                this._settings.Title = this._settings.Email;
        }
    }
}
