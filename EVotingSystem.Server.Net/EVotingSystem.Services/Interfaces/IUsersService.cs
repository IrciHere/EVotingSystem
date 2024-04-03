using EVotingSystem.Contracts.Login;
using EVotingSystem.Contracts.User;

namespace EVotingSystem.Services.Interfaces;

public interface IUsersService
{
    Task<UserDto> CreateUserAsync(NewUserDto user);
    Task RequestForgotPassword(string email);
    /// <summary>
    /// A method used for setting the password first time, or when it was forgotten
    /// </summary>
    /// <param name="resetPasswordDto"></param>
    /// <returns>Returns true is password was changed, and false if it was not (the reset code was incorrect)</returns>
    Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto);
}