using AutoMapper;
using EVotingSystem.Contracts.Election;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Interfaces;
using EVotingSystem.Services.Interfaces.Helpers;

namespace EVotingSystem.Services.Implementations;

public class ElectionService : IElectionService
{
    private readonly IElectionRepository _electionRepository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly IMapper _mapper;

    public ElectionService(IElectionRepository electionRepository, IPasswordEncryptionService passwordEncryptionService, IMapper mapper)
    {
        _electionRepository = electionRepository;
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
}