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

    /// <summary>
    /// A method used for changing the password, if the user knows current one
    /// </summary>
    /// <param name="changePasswordDto"></param>
    /// <param name="userId">Retrieved from JWT token</param>
    /// <returns>Returns true is password was changed, and false if it was not (the current password)</returns>
    Task<bool> ChangePassword(ChangePasswordDto changePasswordDto, string userId);
}