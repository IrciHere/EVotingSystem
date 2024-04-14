using EVotingSystem.Services.Interfaces;

namespace EVotingSystem.Services.Implementations;

public class SmsService : ISmsService
{
    public Task SendOtpCote(string phoneNumber, string otpCode)
    {
        return Task.CompletedTask;
    }
}