using EVotingSystem.Contracts.Election;
using EVotingSystem.Contracts.User;

namespace EVotingSystem.Services.Interfaces;

public interface IElectionService
{
    Task<List<ElectionDto>> GetAllElections();
    Task<List<ElectionDto>> GetElectionsForUser(string userId);
    Task<List<CandidateDto>> GetElectionCandidates(int electionId);
    Task<List<ElectionResultDto>> GetElectionResults(int electionId);
    Task<ElectionDto> CreateElection(NewElectionDto newElection);
    Task<ElectionDto> AssignCandidates(int electionId, List<NewUserDto> users);
    Task<ElectionDto> AssignEligibleVoters(int electionId, List<NewUserDto> users);
    Task<ElectionDto> FinalizeElection(int electionId);
}