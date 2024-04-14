using EVotingSystem.Database.Entities;

namespace EVotingSystem.Repositories.Interfaces;

public interface IVotesRepository
{
    Task<ElectionVote> AddVote(ElectionVote vote);
}