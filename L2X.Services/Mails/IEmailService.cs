using L2X.Core.Authentications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace L2X.Services.Mails
{
    public interface IEmailService<T> : IEmailSender, IEmailSender<T>
        where T : class, IAuthUser
    {
        Task<bool> Send(string email, string subject, string message);

        Task<bool> ConfirmEmail(T user, string confirmLink);

        Task<bool> ResetPassword(T user, string resetLink);

        Task<bool> SendResetCode(T user, string resetCode);
    }
}