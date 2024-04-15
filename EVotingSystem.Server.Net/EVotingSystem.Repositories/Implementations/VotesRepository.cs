using EVotingSystem.Database.Context;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVotingSystem.Repositories.Implementations;

public class VotesRepository : IVotesRepository
{
    private readonly EVotingDbContext _dbContext;

    public VotesRepository(EVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ElectionVote> GetVoteByHash(int electionId, byte[] voteHash, bool withOtp = false)
    {
        IQueryable<ElectionVote> votes = _dbContext.ElectionVotes
            .Where(ev => ev.ElectionId == electionId);

        if (withOtp)
        {
            votes = votes.Include(v => v.VotesOtp);
        }

        ElectionVote vote = await votes.FirstOrDefaultAsync(ev => ev.VoteHash == voteHash);

        return vote;
    }

    public async Task<ElectionVote> AddVote(ElectionVote vote)
    {
        _dbContext.ElectionVotes.Add(vote);

        await _dbContext.SaveChangesAsync();

        return vote;
    }

    public async Task<ElectionVote> ReplaceVote(ElectionVote voteToBeReplaced, ElectionVote newVote)
    {
        VotesOtp replacedVoteOtp = await _dbContext.VotesOtps
            .FirstOrDefaultAsync(otp => otp.VoteId == voteToBeReplaced.Id);

        if (replacedVoteOtp is not null)
        {
            _dbContext.VotesOtps.Remove(replacedVoteOtp);
        }
        _dbContext.ElectionVotes.Remove(voteToBeReplaced);
        _dbContext.ElectionVotes.Add(newVote);

        await _dbContext.SaveChangesAsync();

        return newVote;
    }

    public async Task<ElectionVote> ValidateVote(ElectionVote vote)
    {
        vote.IsVerified = true;
        
        _dbContext.VotesOtps.Remove(vote.VotesOtp);

        await _dbContext.SaveChangesAsync();

        return vote;
    }
}