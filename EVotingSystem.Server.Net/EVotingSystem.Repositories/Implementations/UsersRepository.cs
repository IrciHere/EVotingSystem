using EVotingSystem.Database.Context;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVotingSystem.Repositories.Implementations;

public class UsersRepository : IUsersRepository
{
    private readonly EVotingDbContext _dbContext;

    public UsersRepository(EVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetUserByEmail(string email, bool withPassword = false, bool withPasswordResetCode = false)
    {
        IQueryable<User> users = _dbContext.Users;

        if (withPassword)
        {
            users = users
                .Include(u => u.UserPassword)
                .Include(u => u.UserSecret);
        }

        if (withPasswordResetCode)
        {
            users = users
                .Include(u => u.PasswordResetCode);
        }

        User user = await users.FirstOrDefaultAsync(u => u.Email == email);

        return user;
    }

    public async Task<User> CreateUser(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        return user;
    }
}