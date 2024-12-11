using L2X.Core.Authentications;
using L2X.Core.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace L2X.Services.Mails
{
    public class EmailService<T> : IEmailService<T>, IEmailSender, IEmailSender<T>
        where T : class, IAuthUser
    {
        private List<Attachment>? _attachments;

        private readonly IConfiguration _config;

        private readonly ILogger _logger;

        public string Sender { get; }

        public string Password { get; }

        public string Host { get; }

        public int Port { get; }

        public EmailTermplate ConfirmTemplate { get; }

        public EmailTermplate PasswordTemplate { get; }

        public EmailTermplate ResetCodeTemplate { get; }

        public EmailService(IConfiguration config, ILoggerFactory logFactory)
        {
            _attachments = null;
            _config = config;
            _logger = logFactory.CreateLogger(GetType());
            Sender = _config["Email:sender"] ?? "";
            Password = _config["Email:password"] ?? "";
            Host = _config["Email:host"] ?? "";
            Port = Util.Convert<string, int?>(_config["Email:sender"]) ?? 587;
            ConfirmTemplate = _config.GetValue<EmailTermplate>("Email:confirm") ?? EmailTermplate.ConfirmTemplate;
            PasswordTemplate = _config.GetValue<EmailTermplate>("Email:password") ?? EmailTermplate.PasswordTemplate;
            ResetCodeTemplate = _config.GetValue<EmailTermplate>("Email:resetcode") ?? EmailTermplate.ResetCodeTemplate;
        }

        #region Overriden
        /// <inheritdoc/>
        async Task IEmailSender.SendEmailAsync(string email, string subject, string message)
            => await Send(email, subject, message);

        /// <inheritdoc/>
        async Task IEmailSender<T>.SendConfirmationLinkAsync(T user, string email, string confirmLink)
            => await ConfirmEmail(user, confirmLink);

        /// <inheritdoc/>
        async Task IEmailSender<T>.SendPasswordResetLinkAsync(T user, string email, string resetLink)
            => await ResetPassword(user, resetLink);

        /// <inheritdoc/>
        async Task IEmailSender<T>.SendPasswordResetCodeAsync(T user, string email, string resetCode)
            => await SendResetCode(user, resetCode);

        public async Task Attach(string fileName, string mediaType = "")
            => (_attachments ??= []).Add(Util.IsEmpty(mediaType) ? new(fileName) : new(fileName, mediaType));

        public async Task Attach(Stream content, string fileName, string mediaType)
            => (_attachments ??= []).Add(Util.IsEmpty(mediaType) ? new(content, fileName) : new(content, fileName, mediaType));

        public async Task<bool> Send(string email, string? subject, string? message)
        {
            try
            {
                using var mail = new MailMessage(Sender, email);
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                if (!Util.IsEmpty(_attachments))
                {
                    foreach (var a in _attachments)
                    {
                        mail.Attachments.Add(a);
                    }

                    _attachments.Clear();
                    _attachments = null;
                }

                var credential = new NetworkCredential(Sender, Password);
                var smtp = new SmtpClient
                {
                    Host = Host,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = credential,
                    Port = Port,
                };

                await smtp.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }

            return false;
        }

        public async Task<bool> Send(T user, EmailTermplate template, params string[] args)
        {
            var lst = new string[args.Length + 1];
            lst[0] = user.Username ?? "";
            args.CopyTo(lst, 1);

            return await Send(user.Email ?? "", Util.IsEmpty(template.Subject) ? "" : string.Format(template.Subject, user.Username), Util.IsEmpty(template.Message) ? "" : string.Format(template.Message, lst));
        }

        public async Task<bool> ConfirmEmail(T user, string confirmLink)
            => await Send(user, ConfirmTemplate, confirmLink);

        public async Task<bool> ResetPassword(T user, string resetLink)
            => await Send(user, PasswordTemplate, resetLink);

        public async Task<bool> SendResetCode(T user, string resetCode)
            => await Send(user, ResetCodeTemplate, resetCode);
        #endregion
    }
}