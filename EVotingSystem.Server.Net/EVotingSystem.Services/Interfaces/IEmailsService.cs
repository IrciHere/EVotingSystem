using EVotingSystem.Database.Entities;

namespace EVotingSystem.Services.Interfaces;

public interface IEmailsService
{
    Task SendResetPasswordMail(string email, string code);
    Task SendManyPasswordResetMails(List<User> users);
}