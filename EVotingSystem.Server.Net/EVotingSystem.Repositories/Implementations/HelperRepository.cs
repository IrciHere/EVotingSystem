using EVotingSystem.Database.Context;
using EVotingSystem.Repositories.Interfaces;

namespace EVotingSystem.Repositories.Implementations;

public class HelperRepository : IHelperRepository
{
    private readonly EVotingDbContext _dbContext;

    public HelperRepository(EVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}