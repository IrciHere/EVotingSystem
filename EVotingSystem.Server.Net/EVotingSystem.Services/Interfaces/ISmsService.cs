namespace EVotingSystem.Services.Interfaces;

public interface ISmsService
{
    Task SendOtpCote(string phoneNumber, string otpCode);
}