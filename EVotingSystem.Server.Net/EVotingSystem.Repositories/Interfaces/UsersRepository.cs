using EVotingSystem.Database.Context;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Implementations;

namespace EVotingSystem.Repositories.Interfaces;

public class UsersRepository : IUsersRepository
{
    private readonly EVotingDbContext _dbContext;

    public UsersRepository(EVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateUser(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        return user;
    }
}