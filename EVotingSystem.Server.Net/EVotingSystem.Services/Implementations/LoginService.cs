using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EVotingSystem.Contracts.Login;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Interfaces;
using EVotingSystem.Services.Interfaces.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace EVotingSystem.Services.Implementations;

public class LoginService : ILoginService
{
    #warning GET THIS OUT OF HERE
    private const string TokenSecret = "KeyToBeMovedLaterInDevelopmentProcess";
    private const string Issuer = "localhost";
    private const string Audience = "localhost";
    
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;

    public LoginService(IUsersRepository usersRepository, IPasswordEncryptionService passwordEncryptionService)
    {
        _usersRepository = usersRepository;
        _passwordEncryptionService = passwordEncryptionService;
    }

    public async Task<string> LoginUser(LoginDto login)
    {
        User user = await _usersRepository.GetUserByEmail(login.Email, withPassword: true);

        if (user is null)
        {
            return string.Empty;
        }

        if (!IsPasswordCorrectForUser(login.Password, user))
        {
            return string.Empty;
        }

        string token = GenerateToken(user.Id);

        return token;
    }

    private string GenerateToken(int userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSecret));

        var claims = new List<Claim>()
        {
            new("UserId", userId.ToString())
        };

        var signingCredetials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            Issuer,
            Audience,
            claims, 
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: signingCredetials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool IsPasswordCorrectForUser(string passwordProvided, User user)
    {
        byte[] hashedProvidedPassword =
            _passwordEncryptionService.HashSHA256WithSalt(passwordProvided, user.UserSecret.PasswordSalt);

        return hashedProvidedPassword.SequenceEqual(user.UserPassword.PasswordHash);
    }
}