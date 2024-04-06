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

    public async Task<List<User>> GetAllUsers()
    {
        List<User> users = await _dbContext.Users.ToListAsync();

        return users;
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

    public async Task<User> GetUserById(int userId, bool withPassword = false)
    {
        IQueryable<User> users = _dbContext.Users;

        if (withPassword)
        {
            users = users
                .Include(u => u.UserPassword)
                .Include(u => u.UserSecret);
        }

        User user = await users.FirstOrDefaultAsync(u => u.Id == userId);

        return user;
    }

    public async Task<User> CreateUser(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        
        return user;
    }

    public async Task CreateManyUsers(List<User> users)
    {
        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> GetUserByPasswordResetCode(string resetCode)
    {
        PasswordResetCode userPasswordResetCode = await _dbContext.PasswordResetCodes
            .Include(prc => prc.User)
            .ThenInclude(u => u.UserPassword)
            .Include(prc => prc.User)
            .ThenInclude(u => u.UserSecret)
            .FirstOrDefaultAsync(prc => prc.ResetCode == resetCode);

        return userPasswordResetCode?.User;
    }

    public void RemovePasswordResetCode(PasswordResetCode passwordResetCode)
    {
        _dbContext.PasswordResetCodes.Remove(passwordResetCode);
    }
}