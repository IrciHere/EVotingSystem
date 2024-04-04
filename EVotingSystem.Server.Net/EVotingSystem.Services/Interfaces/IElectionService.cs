using EVotingSystem.Contracts.Election;

namespace EVotingSystem.Services.Interfaces;

public interface IElectionService
{
    Task<List<ElectionDto>> GetAllElections();
    Task<ElectionDto> CreateElection(NewElectionDto newElection);
}