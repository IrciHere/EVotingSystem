using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EVotingSystem.Contracts.Login;
using EVotingSystem.Database.Entities;
using EVotingSystem.Models;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Interfaces;
using EVotingSystem.Services.Interfaces.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EVotingSystem.Services.Implementations;

public class LoginService : ILoginService
{
    private readonly string _tokenSecret;
    private readonly string _issuer;
    private readonly string _audience;

    private readonly IHelperService _helperService;
    private readonly IUsersRepository _usersRepository;

    public LoginService(IConfiguration configuration, IUsersRepository usersRepository, IHelperService helperService)
    {
        _tokenSecret = configuration["JwtSettings:Key"];
        _issuer = configuration["JwtSettings:Issuer"];
        _audience = configuration["JwtSettings:Audience"];
        _usersRepository = usersRepository;
        _helperService = helperService;
    }

    public async Task<string> LoginUser(LoginDto login)
    {
        User user = await _usersRepository.GetUserByEmail(login.Email, withPassword: true);

        if (!_helperService.IsPasswordCorrectForUser(login.Password, user))
        {
            return string.Empty;
        }

        string token = GenerateToken(user);

        return token;
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSecret));

        var claims = new List<Claim>
        {
            new("UserId", user.Id.ToString()),
            new(ClaimTypes.Role, user.IsAdmin ? Roles.Admin : Roles.User)
        };

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims, 
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}