using EVotingSystem.Database.Entities;
using EVotingSystem.Services.Interfaces.Helpers;

namespace EVotingSystem.Services.Implementations.Helpers;

public class HelperService : IHelperService
{
    private readonly IPasswordEncryptionService _passwordEncryptionService;

    public HelperService(IPasswordEncryptionService passwordEncryptionService)
    {
        _passwordEncryptionService = passwordEncryptionService;
    }

    public bool IsPasswordCorrectForUser(string passwordProvided, User user)
    {
        if (user?.UserSecret?.PasswordSalt is null)
        {
            return false;
        }
        
        byte[] hashedProvidedPassword =
            _passwordEncryptionService.HashSHA256WithSalt(passwordProvided, user.UserSecret.PasswordSalt);

        return hashedProvidedPassword.SequenceEqual(user.UserPassword.PasswordHash);
    }
}