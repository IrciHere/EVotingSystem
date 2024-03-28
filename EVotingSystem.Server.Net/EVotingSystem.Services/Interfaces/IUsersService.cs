using EVotingSystem.Contracts.User;

namespace EVotingSystem.Services.Interfaces;

public interface IUsersService
{
    Task<NewUserDto> CreateUserAsync(NewUserDto user);
}