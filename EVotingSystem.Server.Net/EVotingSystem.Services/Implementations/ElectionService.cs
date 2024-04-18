using System.Text.Json;
using AutoMapper;
using EVotingSystem.Contracts.Election;
using EVotingSystem.Contracts.User;
using EVotingSystem.Database.Entities;
using EVotingSystem.Models;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Interfaces;
using EVotingSystem.Services.Interfaces.Helpers;

namespace EVotingSystem.Services.Implementations;

public class ElectionService : IElectionService
{
    private readonly IElectionRepository _electionRepository;
    private readonly IHelperRepository _helperRepository;
    private readonly IUsersService _usersService;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly IVotesRepository _votesRepository;
    private readonly IMapper _mapper;

    public ElectionService(IElectionRepository electionRepository, 
        IHelperRepository helperRepository,
        IUsersService usersService,
        IPasswordEncryptionService passwordEncryptionService, 
        IMapper mapper, IVotesRepository votesRepository)
    {
        _electionRepository = electionRepository;
        _helperRepository = helperRepository;
        _usersService = usersService;
        _passwordEncryptionService = passwordEncryptionService;
        _mapper = mapper;
        _votesRepository = votesRepository;
    }

    public async Task<List<ElectionDto>> GetAllElections()
    {
        List<Election> elections = await _electionRepository.GetAllElections();

        var mappedElections = _mapper.Map<List<ElectionDto>>(elections);

        return mappedElections;
    }

    public async Task<List<ElectionDto>> GetElectionsForUser(string userId)
    {
        int userIdNumeric = int.Parse(userId);
        
        List<Election> elections = await _electionRepository.GetElectionsForUser(userIdNumeric);
        
        var mappedElections = _mapper.Map<List<ElectionDto>>(elections);

        return mappedElections;
    }

    public async Task<List<CandidateDto>> GetElectionCandidates(int electionId)
    {
        Election election = await _electionRepository.GetElectionById(electionId, withCandidates: true);

        if (election is null)
        {
            return [];
        }

        List<User> candidates = election.Candidates.ToList();

        var mappedCandidates = _mapper.Map<List<CandidateDto>>(candidates);

        return mappedCandidates;
    }

    public async Task<List<ElectionResultDto>> GetElectionResults(int electionId)
    {
        Election election = await _electionRepository.GetElectionById(electionId);

        if (election is null)
        {
            return [];
        }

        if (!election.HasEnded)
        {
            return [];
        }

        List<ElectionResult> results = await _electionRepository.GetElectionResults(electionId);

        var resultsMapped = _mapper.Map<List<ElectionResultDto>>(results);

        return resultsMapped;
    }

    public async Task<ElectionDto> CreateElection(NewElectionDto newElection)
    {
        var election = _mapper.Map<Election>(newElection);

        byte[] electionSecret = _passwordEncryptionService.GenerateRandomByteArray();
        election.ElectionSecret = new ElectionSecret { Secret = electionSecret };

        await _electionRepository.CreateElection(election);

        var mappedElection = _mapper.Map<ElectionDto>(election);

        return mappedElection;
    }

    public async Task<ElectionDto> AssignCandidates(int electionId, List<NewUserDto> users)
    {
        Election election = await _electionRepository.GetElectionById(electionId);

        if (election is null || election.HasEnded)
        {
            return null;
        }

        var candidatesToAdd = _mapper.Map<List<User>>(users);

        await _usersService.CreateUsersOrAssignExistingEntities(candidatesToAdd);

        election.Candidates = candidatesToAdd;

        await _helperRepository.SaveChangesAsync();

        var mappedElection = _mapper.Map<ElectionDto>(election);

        return mappedElection;
    }

    public async Task<ElectionDto> AssignEligibleVoters(int electionId, List<NewUserDto> users)
    {
        Election election = await _electionRepository.GetElectionById(electionId);

        if (election is null || election.HasEnded)
        {
            return null;
        }

        var votersToAdd = _mapper.Map<List<User>>(users);

        await _usersService.CreateUsersOrAssignExistingEntities(votersToAdd);

        List<EligibleVoter> eligibleVoters = votersToAdd.Select(v => new EligibleVoter
        {
            ElectionId = electionId,
            UserId = v.Id,
            HasVoted = false
        }).ToList();

        election.EligibleVoters = eligibleVoters;

        await _helperRepository.SaveChangesAsync();

        var mappedElection = _mapper.Map<ElectionDto>(election);

        return mappedElection;
    }

    public async Task<ElectionDto> FinalizeElection(int electionId)
    {
        Election election = await _electionRepository.GetElectionById(electionId, withSecret: true, withCandidates: true);

        if (election is null)
        {
            return null;
        }

        if (election.EndTime.AddMinutes(10) > DateTime.Now || election.HasEnded)
        {
            return null;
        }
        
        // get votes
        List<ElectionVote> votes = await _votesRepository.GetAllVotesForElection(electionId);
        
        // decrypt votes
        byte[] ivArray = _passwordEncryptionService.GenerateIVArrayFromUserId(electionId);
        
        foreach (ElectionVote vote in votes)
        {
            string decryptedVoteStr = _passwordEncryptionService
                .DecryptSecret(vote.VotedCandidateEncrypted, election.ElectionSecret.Secret, ivArray);
            var decryptedVote = JsonSerializer.Deserialize<VoteEncryptModel>(decryptedVoteStr);
            vote.VotedCandidateId = decryptedVote.CandidateId;
        }
        
        // create summarised results
        Dictionary<int?, List<ElectionVote>> resultsGrouped = votes
            .GroupBy(v => v.VotedCandidateId)
            .ToDictionary(g => g.Key, g => g.ToList());
        
        List<ElectionResult> results = election.Candidates.Select(c => new ElectionResult
            {
                ElectionId = electionId,
                UserId = c.Id,
                Votes = resultsGrouped.TryGetValue(c.Id, out List<ElectionVote> value) ? value.Count : 0
            })
            .ToList();
        
        election.ElectionResults = results;
        
        election.HasEnded = true;
        
        await _helperRepository.SaveChangesAsync();

        await _votesRepository.RemoveUnvalidatedVotesForElection(electionId);

        var mappedElection = _mapper.Map<ElectionDto>(election);
        return mappedElection;
    }
}