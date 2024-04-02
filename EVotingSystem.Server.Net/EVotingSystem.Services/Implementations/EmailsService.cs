using EVotingSystem.Services.Interfaces;

namespace EVotingSystem.Services.Implementations;

public class EmailsService : IEmailsService
{
    public Task SendResetPasswordMail(string email, string code)
    {
        // TODO: implement later
        return Task.CompletedTask;
    }
}