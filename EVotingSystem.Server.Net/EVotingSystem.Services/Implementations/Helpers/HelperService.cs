using EVotingSystem.Database.Entities;
using EVotingSystem.Services.Interfaces.Helpers;

namespace EVotingSystem.Services.Implementations.Helpers;

public class HelperService : IHelperService
{
    private readonly IEncryptionService _encryptionService;

    public HelperService(IEncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    public bool IsPasswordCorrectForUser(string passwordProvided, User user)
    {
        if (user?.UserSecret?.PasswordSalt is null)
        {
            return false;
        }
        
        byte[] hashedProvidedPassword =
            _encryptionService.HashSHA256WithSalt(passwordProvided, user.UserSecret.PasswordSalt);

        return hashedProvidedPassword.SequenceEqual(user.UserPassword.PasswordHash);
    }
}