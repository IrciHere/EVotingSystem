using System.Security.Cryptography;
using System.Text;
using EVotingSystem.Services.Interfaces.Helpers;

namespace EVotingSystem.Services.Implementations.Helpers;

public class PasswordEncryptionService : IPasswordEncryptionService
{
    public byte[] HashPasswordWithSalt(string password, byte[] salt)
    {
        byte[] saltedPassword = Encoding.UTF8
            .GetBytes(password)
            .Concat(salt)
            .ToArray();

        byte[] hashedPassword = SHA256.HashData(saltedPassword);

        return hashedPassword;
    }

    public byte[] GeneratePasswordSalt()
    {
        var rng = RandomNumberGenerator.Create();
        var salt = new byte[32];
             
        rng.GetBytes(salt);

        return salt;
    }
}