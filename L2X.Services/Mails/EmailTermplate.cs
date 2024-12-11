namespace L2X.Services.Mails;

public class EmailTermplate
{
    public static EmailTermplate ConfirmTemplate
        => new()
        {
            EmailList = "",
            Subject = "Confirm your email registration",
            Message = "<p>Hello {0},</p>" +
                    "<p>You received this email because your email address is used to register a new member account on our system!</p>" +
                    "<p>Please follow this link <a href=\"{1}\">{1}</a> to complete your registration!</p><p>Thank you.</p>"
        };

    public static EmailTermplate PasswordTemplate
        => new()
        {
            EmailList = "",
            Subject = "Request to reset password",
            Message = "<p>Hello {0},</p>" +
                    "<p>You have requested our system to reset password for your acccount!</p>" +
                    "<p>Please follow this link <a href=\"{1}\">{1}</a> to reset your password!</p>"
        };

    public static EmailTermplate ResetCodeTemplate
        => new()
        {
            EmailList = "",
            Subject = "Reset password code",
            Message = "<p>Hello {0},</p>" +
                    "<p>You have completed the process to reset your password!</p>" +
                    "<p>Your reset password code is: <b>{1}</b></p>" +
                    "<p>Please use this code to reset your password!</p>"
        };

    public string? EmailList { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }
}