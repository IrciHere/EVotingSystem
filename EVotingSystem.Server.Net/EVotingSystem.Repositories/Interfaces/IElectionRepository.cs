using EVotingSystem.Database.Entities;

namespace EVotingSystem.Repositories.Interfaces;

public interface IElectionRepository
{
    Task<Election> GetElectionById(int electionId, bool withSecret = false, bool withVoters = false, bool withCandidates = false);
    Task<List<Election>> GetAllElections();
    Task<Election> CreateElection(Election election);
}