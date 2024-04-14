using EVotingSystem.Database.Context;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Interfaces;

namespace EVotingSystem.Repositories.Implementations;

public class VotesRepository : IVotesRepository
{
    private readonly EVotingDbContext _dbContext;

    public VotesRepository(EVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ElectionVote> AddVote(ElectionVote vote)
    {
        _dbContext.ElectionVotes.Add(vote);

        await _dbContext.SaveChangesAsync();

        return vote;
    }
}