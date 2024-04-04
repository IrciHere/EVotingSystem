using EVotingSystem.Database.Entities;

namespace EVotingSystem.Repositories.Interfaces;

public interface IElectionRepository
{
    Task<List<Election>> GetAllElections();
    Task<Election> CreateElection(Election election);
}