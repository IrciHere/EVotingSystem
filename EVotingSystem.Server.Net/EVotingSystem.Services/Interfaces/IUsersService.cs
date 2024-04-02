using EVotingSystem.Contracts.User;

namespace EVotingSystem.Services.Interfaces;

public interface IUsersService
{
    Task<UserDto> CreateUserAsync(NewUserDto user);
    Task RequestForgotPassword(string email);
}