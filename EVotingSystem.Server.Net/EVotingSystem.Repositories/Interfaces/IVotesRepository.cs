using EVotingSystem.Database.Entities;

namespace EVotingSystem.Repositories.Interfaces;

public interface IVotesRepository
{
    Task<ElectionVote> GetVoteByHash(int electionId, byte[] voteHash, bool withOtp = false);
    Task<ElectionVote> AddVote(ElectionVote vote);
    Task<ElectionVote> ReplaceVote(ElectionVote voteToBeReplaced, ElectionVote newVote);
    Task<ElectionVote> ValidateVote(ElectionVote vote);
}