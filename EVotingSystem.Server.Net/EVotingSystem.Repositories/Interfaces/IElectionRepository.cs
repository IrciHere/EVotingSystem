﻿using EVotingSystem.Database.Entities;

namespace EVotingSystem.Repositories.Interfaces;

public interface IElectionRepository
{
    Task<Election> GetElectionById(int electionId);
    Task<List<Election>> GetAllElections();
    Task<Election> CreateElection(Election election);
}