using EVotingSystem.Contracts.Vote;

namespace EVotingSystem.Services.Interfaces;

public interface IVotesService
{
    Task<List<VoteDto>> GetAllVotesForElection(int electionId);
    Task<byte[]> GetMyVoteHashForElection(string userId, HashCheckDto hashCheckDto);
    Task<byte[]> Vote(string voterId, InputVoteDto vote);
    Task<byte[]> ValidateVote(ValidateVoteDto validateVoteDto);
}