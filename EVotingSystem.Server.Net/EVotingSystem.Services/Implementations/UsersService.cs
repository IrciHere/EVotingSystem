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
    private readonly IHelperService _helperService;
    private readonly IUsersRepository _usersRepository;
    private readonly IHelperRepository _helperRepository;
    private readonly IEmailsService _emailsService;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly IMapper _mapper;

    public UsersService(IUsersRepository usersRepository, IHelperRepository helperRepository, IEmailsService emailsService, 
        IPasswordEncryptionService passwordEncryptionService, IMapper mapper, IHelperService helperService)
    {
        _usersRepository = usersRepository;
        _helperRepository = helperRepository;
        _emailsService = emailsService;
        _passwordEncryptionService = passwordEncryptionService;
        _mapper = mapper;
        _helperService = helperService;
    }

    public async Task<UserDto> CreateUser(NewUserDto user)
    {
        var newUserEntity = _mapper.Map<User>(user);
        
        AddPasswordResetCodeToUser(newUserEntity);

        newUserEntity = await _usersRepository.CreateUser(newUserEntity);

        _emailsService.SendResetPasswordMail(newUserEntity.Email, newUserEntity.PasswordResetCode.ResetCode);

        var newUser = _mapper.Map<UserDto>(newUserEntity);

        return newUser;
    }

    public async Task CreateUsersOrAssignExistingEntities(List<User> users)
    {
        List<User> existingUsers = await _usersRepository.GetAllUsers();

        List<User> newUsers = [];

        for (var i = 0; i < users.Count; i++)
        {
            User user = users[i];
            User existingUser = existingUsers.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser is not null)
            {
                users[i] = existingUser;
            }
            else
            {
                AddPasswordResetCodeToUser(user);
                newUsers.Add(user);
            }
        }

        await _usersRepository.CreateManyUsers(newUsers);
        _emailsService.SendManyPasswordResetMails(newUsers);
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

        _emailsService.SendResetPasswordMail(user.Email, user.PasswordResetCode.ResetCode);
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

        user.UserSecret.PasswordSalt = _passwordEncryptionService.GenerateRandomByteArray();

        const int votingSecretLength = 100;
        string userVotingSecret = GenerateRandomText(votingSecretLength);

        SetUserVotingSecretAndPassword(userVotingSecret, resetPasswordDto.NewPassword, user);

        await _helperRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ChangePassword(ChangePasswordDto changePasswordDto, string userId)
    {
        User user = await _usersRepository.GetUserById(int.Parse(userId), withPassword: true);

        if (!_helperService.IsPasswordCorrectForUser(changePasswordDto.OldPassword, user))
        {
            return false;
        }

        SetNewUserVotingSecretAndPassword(changePasswordDto.OldPassword, changePasswordDto.NewPassword, user);

        await _helperRepository.SaveChangesAsync();

        return true;
    }

    private void SetUserVotingSecretAndPassword(string votingSecret, string newPassword, User user)
    {
        // SHA256 hash of password without salt is used as AES encryption key
        byte[] votingSecretEncryptionKey = _passwordEncryptionService.HashSHA256(newPassword);
        byte[] votingSecretEncryptionIV = _passwordEncryptionService.GenerateIVArrayFromUserId(user.Id);
        byte[] votingSecretEncrypted = _passwordEncryptionService.EncryptSecret(votingSecret, votingSecretEncryptionKey, votingSecretEncryptionIV);

        user.UserSecret.VotingSecret = votingSecretEncrypted;

        byte[] hashedPassword =
            _passwordEncryptionService.HashSHA256WithSalt(newPassword, user.UserSecret.PasswordSalt);

        user.UserPassword ??= new UserPassword();

        user.UserPassword.PasswordHash = hashedPassword;
    }

    private void SetNewUserVotingSecretAndPassword(string oldPassword, string newPassword, User user)
    {
        // re-encrypt secret
        byte[] votingSecretEncryptionIV = _passwordEncryptionService.GenerateIVArrayFromUserId(user.Id);
        
        // SHA256 hash of password without salt is used as AES encryption key
        byte[] oldVotingSecretEncryptionKey = _passwordEncryptionService.HashSHA256(oldPassword);
        string votingSecretDecrypted = _passwordEncryptionService.DecryptSecret(user.UserSecret.VotingSecret, oldVotingSecretEncryptionKey, votingSecretEncryptionIV);
        
        SetUserVotingSecretAndPassword(votingSecretDecrypted, newPassword, user);
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