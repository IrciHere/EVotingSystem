namespace EVotingSystem.Services.Interfaces.Helpers;

public interface IPasswordEncryptionService
{
    byte[] HashPasswordWithSalt(string password, byte[] salt);
    byte[] GeneratePasswordSalt();
}