using EVotingSystem.Database.Entities;

namespace EVotingSystem.Repositories.Interfaces;

public interface IUsersRepository
{
    Task<User> GetUserByEmail(string email, bool withPassword = false, bool withPasswordResetCode = false);
    Task<User> CreateUser(User user);
}