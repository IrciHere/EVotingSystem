using EVotingSystem.Database.Context;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVotingSystem.Repositories.Implementations;

public class ElectionRepository : IElectionRepository
{
    private readonly EVotingDbContext _dbContext;

    public ElectionRepository(EVotingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Election> GetElectionById(int electionId, bool withSecret = false, bool withVoters = false, bool withCandidates = false)
    {
        IQueryable<Election> elections = _dbContext.Elections;

        if (withSecret)
        {
            elections = elections.Include(e => e.ElectionSecret);
        }

        if (withVoters)
        {
            elections = elections.Include(e => e.EligibleVoters);
        }

        if (withCandidates)
        {
            elections = elections.Include(e => e.Candidates);
        }
        
        Election election = await  elections
            .FirstOrDefaultAsync(e => e.Id == electionId);

        return election;
    }

    public async Task<List<ElectionResult>> GetElectionResults(int electionId)
    {
        List<ElectionResult> results = await _dbContext.ElectionResults
            .Where(er => er.ElectionId == electionId)
            .Include(er => er.User)
            .OrderByDescending(er => er.Votes)
            .ToListAsync();

        return results;
    }

    public async Task<List<Election>> GetAllElections()
    {
        List<Election> elections = await _dbContext.Elections.ToListAsync();

        return elections;
    }

    public async Task<List<Election>> GetElectionsForUser(int userId)
    {
        List<Election> elections = await _dbContext.EligibleVoters
            .Where(ev => ev.UserId == userId)
            .Include(ev => ev.Election)
            .Select(ev => ev.Election)
            .ToListAsync();

        return elections;
    }

    public async Task<Election> CreateElection(Election election)
    {
        _dbContext.Elections.Add(election);
        await _dbContext.SaveChangesAsync();

        return election;
    }
}