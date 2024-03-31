using EVotingSystem.Database.Entities;

namespace EVotingSystem.Repositories.Implementations;

public interface IUsersRepository
{
    Task<User> CreateUser(User user);
}