using EVotingSystem.Services.Interfaces;

namespace EVotingSystem.Services.Implementations.Sms;

public class MockSmsService : ISmsService
{
    public Task SendOtpCote(string phoneNumber, string otpCode)
    {
        return Task.CompletedTask;
    }
}