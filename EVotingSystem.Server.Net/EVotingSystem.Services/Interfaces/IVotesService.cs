using EVotingSystem.Contracts.Vote;

namespace EVotingSystem.Services.Interfaces;

public interface IVotesService
{
    Task Vote(string voterId, InputVoteDto vote);
}