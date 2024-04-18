using EVotingSystem.Database.Entities;

namespace EVotingSystem.Services.Interfaces;

public interface IEmailsService
{
    void SendResetPasswordMail(string email, string code);
    void SendManyPasswordResetMails(List<User> users);
}