using System.Net;
using System.Net.Mail;
using System.Text;
using EVotingSystem.Database.Entities;
using EVotingSystem.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EVotingSystem.Services.Implementations.Emails;

public class GmailEmailsService : IEmailsService
{
    private const string SmtpServer = "smtp.gmail.com";
    private readonly string _mailserverLogin;
    private readonly string _mailServerPassword;
    private readonly string _mailUserName;

    public GmailEmailsService(IConfiguration configuration)
    {
        _mailserverLogin = configuration["GmailSettings:ServerLogin"];
        _mailServerPassword = configuration["GmailSettings:ServerPassword"];
        _mailUserName = configuration["GmailSettings:UserName"];
    }
    
    public void SendResetPasswordMail(string email, string code)
    {
        SmtpClient client = CreateClient();

        SendMail(email, code, client);
    }

    public void SendManyPasswordResetMails(List<User> users)
    {
        SmtpClient client = CreateClient();

        foreach (User user in users)
        {
            SendMail(user.Email, user.PasswordResetCode.ResetCode, client);
        }
    }
    
    
    private void SendMail(string email, string code, SmtpClient client)
    {
        var from = new MailAddress(_mailserverLogin, _mailUserName, Encoding.UTF8);
        var to = new MailAddress(email);

        var mailMessage = new MailMessage(from, to)
        {
            Body = $"Zresetuj swoje hasło używając kodu: {code}",
            Subject = "E-Voting System - Reset hasła"
        };

        try
        {
            client.Send(mailMessage);
        }
        catch (SmtpException ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private SmtpClient CreateClient()
    {
        return new SmtpClient(SmtpServer)
        {
            Port = 587,
            Credentials = new NetworkCredential(_mailserverLogin, _mailServerPassword),
            EnableSsl = true
        };
    }
}