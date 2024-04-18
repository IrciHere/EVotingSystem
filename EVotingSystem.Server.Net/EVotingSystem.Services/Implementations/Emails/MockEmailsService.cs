using EVotingSystem.Database.Entities;
using EVotingSystem.Services.Interfaces;

namespace EVotingSystem.Services.Implementations.Emails;

public class MockEmailsService : IEmailsService
{
    public void SendResetPasswordMail(string email, string code)
    {
        // Mock - do nothing
    }

    public void SendManyPasswordResetMails(List<User> users)
    {
        // Mock - do nothing
    }
}