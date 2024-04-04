using EVotingSystem.Database.Entities;

namespace EVotingSystem.Repositories.Interfaces;

public interface IUsersRepository
{
    Task<User> GetUserByEmail(string email, bool withPassword = false, bool withPasswordResetCode = false);
    Task<User> GetUserById(int userId, bool withPassword = false);
    Task<User> CreateUser(User user);
    Task<User> GetUserByPasswordResetCode(string resetCode);
    void RemovePasswordResetCode(PasswordResetCode passwordResetCode);
}