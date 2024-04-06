using EVotingSystem.Contracts.Election;
using EVotingSystem.Contracts.User;

namespace EVotingSystem.Services.Interfaces;

public interface IElectionService
{
    Task<List<ElectionDto>> GetAllElections();
    Task<ElectionDto> CreateElection(NewElectionDto newElection);
    Task<ElectionDto> AssignCandidates(int electionId, List<NewUserDto> users);
}