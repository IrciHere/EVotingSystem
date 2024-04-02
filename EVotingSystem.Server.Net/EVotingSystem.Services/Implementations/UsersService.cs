using System.Security.Cryptography;
using AutoMapper;
using EVotingSystem.Contracts.User;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Interfaces;

namespace EVotingSystem.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IHelperRepository _helperRepository;
    private readonly IEmailsService _emailsService;
    private readonly IMapper _mapper;

    public UsersService(IUsersRepository usersRepository, IHelperRepository helperRepository, IEmailsService emailsService, IMapper mapper)
    {
        _usersRepository = usersRepository;
        _helperRepository = helperRepository;
        _emailsService = emailsService;
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

    private void AddPasswordResetCodeToUser(User user)
    {
        string resetCode = GeneratePasswordResetCode();

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

    private static string GeneratePasswordResetCode()
    {
        const int codeLength = 10;
        const string possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        
        string resetCode = RandomNumberGenerator.GetString(possibleChars, codeLength);

        return resetCode;
    }
}