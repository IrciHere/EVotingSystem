using System.Security.Cryptography;
using AutoMapper;
using EVotingSystem.Contracts.Login;
using EVotingSystem.Contracts.User;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Interfaces;
using EVotingSystem.Services.Interfaces.Helpers;

namespace EVotingSystem.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IHelperRepository _helperRepository;
    private readonly IEmailsService _emailsService;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly IMapper _mapper;

    public UsersService(IUsersRepository usersRepository, IHelperRepository helperRepository, IEmailsService emailsService, 
        IPasswordEncryptionService passwordEncryptionService, IMapper mapper)
    {
        _usersRepository = usersRepository;
        _helperRepository = helperRepository;
        _emailsService = emailsService;
        _passwordEncryptionService = passwordEncryptionService;
        _mapper = mapper;
    }

    public async Task<UserDto> CreateUserAsync(NewUserDto user)
    {
        var newUserEntity = _mapper.Map<User>(user);
        
        AddPasswordResetCodeToUser(newUserEntity);

        newUserEntity = await _usersRepository.CreateUser(newUserEntity);

        await _emailsService.SendResetPasswordMail(newUserEntity.Email, newUserEntity.PasswordResetCode.ResetCode);

        var newUser = _mapper.Map<UserDto>(newUserEntity);

        return newUser;
    }

    public async Task RequestForgotPassword(string email)
    {
        User user = await _usersRepository.GetUserByEmail(email, withPasswordResetCode: true);

        if (user is null)
        {
            return;
        }
        
        AddPasswordResetCodeToUser(user);

        await _helperRepository.SaveChangesAsync();

        await _emailsService.SendResetPasswordMail(user.Email, user.PasswordResetCode.ResetCode);
    }

    public async Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        User user = await _usersRepository.GetUserByPasswordResetCode(resetPasswordDto.ResetCode);

        if (user is null)
        {
            return false;
        }
        
        _usersRepository.RemovePasswordResetCode(user.PasswordResetCode);

        user.UserSecret ??= new UserSecret();

        user.UserSecret.PasswordSalt = _passwordEncryptionService.GeneratePasswordSalt();

        const int votingSecretLength = 30;
        string userVotingSecret = GenerateRandomText(votingSecretLength);
        byte[] votingSecretHash = _passwordEncryptionService.HashSecret(userVotingSecret);

        user.UserSecret.VotingSecret = votingSecretHash;

        byte[] hashedPassword =
            _passwordEncryptionService.HashPasswordWithSalt(resetPasswordDto.NewPassword, user.UserSecret.PasswordSalt);

        user.UserPassword ??= new UserPassword();

        user.UserPassword.PasswordHash = hashedPassword;

        await _helperRepository.SaveChangesAsync();

        return true;
    }

    private static void AddPasswordResetCodeToUser(User user)
    {
        const int codeLength = 10;
        string resetCode = GenerateRandomText(codeLength);

        if (user.PasswordResetCode is null)
        {
            user.PasswordResetCode = new PasswordResetCode
            {
                ResetCode = resetCode
            };  
        }
        else
        {
            user.PasswordResetCode.ResetCode = resetCode;
        }
    }

    private static string GenerateRandomText(int length)
    {
        const string possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        
        string resetCode = RandomNumberGenerator.GetString(possibleChars, length);

        return resetCode;
    }
}