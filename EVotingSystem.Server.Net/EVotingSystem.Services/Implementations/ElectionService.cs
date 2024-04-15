using AutoMapper;
using EVotingSystem.Contracts.Election;
using EVotingSystem.Contracts.User;
using EVotingSystem.Database.Entities;
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
    private readonly IMapper _mapper;

    public ElectionService(IElectionRepository electionRepository, 
        IHelperRepository helperRepository,
        IUsersService usersService,
        IPasswordEncryptionService passwordEncryptionService, 
        IMapper mapper)
    {
        _electionRepository = electionRepository;
        _helperRepository = helperRepository;
        _usersService = usersService;
        _passwordEncryptionService = passwordEncryptionService;
        _mapper = mapper;
    }

    public async Task<List<ElectionDto>> GetAllElections()
    {
        List<Election> elections = await _electionRepository.GetAllElections();

        var mappedElections = _mapper.Map<List<ElectionDto>>(elections);

        return mappedElections;
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

        if (election is null)
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

        if (election is null)
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
}