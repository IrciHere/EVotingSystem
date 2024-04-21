using System.Security.Cryptography;
using System.Text.Json;
using AutoMapper;
using EVotingSystem.Contracts.Vote;
using EVotingSystem.Database.Entities;
using EVotingSystem.Models;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Interfaces;
using EVotingSystem.Services.Interfaces.Helpers;

namespace EVotingSystem.Services.Implementations;

public class VotesService : IVotesService
{
    private readonly IHelperService _helperService;
    private readonly IElectionRepository _electionRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IVotesRepository _votesRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly ISmsService _smsService;
    private readonly IMapper _mapper;

    public VotesService(IElectionRepository electionRepository, IUsersRepository usersRepository, IVotesRepository votesRepository, IEncryptionService encryptionService, ISmsService smsService, IMapper mapper, IHelperService helperService)
    {
        _electionRepository = electionRepository;
        _usersRepository = usersRepository;
        _votesRepository = votesRepository;
        _encryptionService = encryptionService;
        _smsService = smsService;
        _mapper = mapper;
        _helperService = helperService;
    }

    public async Task<List<VoteDto>> GetAllVotesForElection(int electionId)
    {
        Election election = await _electionRepository.GetElectionById(electionId);

        if (election is null || election.EndTime.AddMinutes(15) > DateTime.Now)
        {
            return [];
        }

        List<ElectionVote> votes = await _votesRepository.GetAllVotesForElection(electionId);

        var mappedVotes = _mapper.Map<List<VoteDto>>(votes);
        return mappedVotes;
    }

    public async Task<byte[]> GetMyVoteHashForElection(string userId, HashCheckDto hashCheckDto)
    {
        int voterIdNumeric = int.Parse(userId);
        User user = await _usersRepository.GetUserById(voterIdNumeric, withPassword: true);

        if (!_helperService.IsPasswordCorrectForUser(hashCheckDto.Password, user))
        {
            return [];
        }
        
        Election election = await _electionRepository
            .GetElectionById(hashCheckDto.ElectionId, withVoters: true);

        if (election?.EligibleVoters is null || !IsUserEligibleVoter(election, voterIdNumeric))
        {
            return [];
        }

        string votingSecretDecrypted =
            _encryptionService.DecryptVotingSecretForUser(user.UserSecret.VotingSecret, hashCheckDto.Password, user.Id);
        
        byte[] voteHash = CreateVoteHash(user.Id, election.Id, votingSecretDecrypted);

        return voteHash;
    }

    public async Task<byte[]> Vote(string voterId, InputVoteDto vote)
    {
        int voterIdNumeric = int.Parse(voterId);
        User user = await _usersRepository.GetUserById(voterIdNumeric, withPassword: true);
        
        if (!_helperService.IsPasswordCorrectForUser(vote.VoterPassword, user))
        {
            return [];
        }

        Election election = await _electionRepository
            .GetElectionById(vote.ElectionId, withSecret: true, withCandidates: true, withVoters: true);

        if (election is null 
            || !IsElectionOngoing(election)
            || !IsUserEligibleVoter(election, voterIdNumeric)
            || !IsUserElectionCandidate(election.Candidates, vote.CandidateId))
        {
            return [];
        }
        
        string votingSecretDecrypted =
            _encryptionService.DecryptVotingSecretForUser(user.UserSecret.VotingSecret, vote.VoterPassword, user.Id);
        
        byte[] voteHash = CreateVoteHash(user.Id, election.Id, votingSecretDecrypted);
        
        ElectionVote previousVote =
            await SetEligibleVoterToVotedAndReturnPreviousVoteIfExists(election, voterIdNumeric, voteHash);
        
        // that case means user has voted, forgot and reset their password and tried to vote again
        if (previousVote is not null && previousVote.Id == 0) 
        {
            return [];
        }
        
        byte[] voteEncrypted = _encryptionService.EncryptVote(vote.CandidateId, vote.ElectionId,
            election.ElectionSecret.Secret, voteHash);

        ElectionVote newVote = await CreateOrReplaceVote(election.Id, voteHash, voteEncrypted, previousVote);
  
        // send SMS
        await _smsService.SendOtpCote(user.PhoneNumber, newVote.VotesOtp.OtpCode);

        return newVote.VoteHash;
    }

    public async Task<byte[]> ValidateVote(ValidateVoteDto validateVoteDto)
    {
        Election election = await _electionRepository.GetElectionById(validateVoteDto.ElectionId);

        if (election is null || !IsElectionOngoing(election, endTimeMargin: 10))
        {
            return [];
        }

        ElectionVote vote = await _votesRepository.GetVoteByHash(election.Id, validateVoteDto.VoteHash, withOtp: true);

        if (vote?.VotesOtp?.OtpCode is null || vote.VotesOtp.OtpCode != validateVoteDto.OtpCode)
        {
            return [];
        }

        await _votesRepository.ValidateVote(vote);

        return vote.VoteHash;
    }
    
    private byte[] CreateVoteHash(int userId, int electionId, string secret)
    {
        var voteHashObject = new VoteHashModel
        {
            UserId = userId,
            ElectionId = electionId,
            Secret = secret
        };

        string voteHashString = JsonSerializer.Serialize(voteHashObject);
        byte[] voteHash = _encryptionService.HashSHA256(voteHashString);

        return voteHash;
    }
    
    private async Task<ElectionVote> SetEligibleVoterToVotedAndReturnPreviousVoteIfExists(Election election, int voterId, byte[] voteHash)
    {
        EligibleVoter voter = election.EligibleVoters.Single(ev => ev.UserId == voterId);
        ElectionVote previousVote = null;

        if (!voter.HasVoted)
        {
            voter.HasVoted = true;
        }
        else
        {
            previousVote = await _votesRepository.GetVoteByHash(election.Id, voteHash) ?? new ElectionVote();
        }

        return previousVote;
    }
    
    private async Task<ElectionVote> CreateOrReplaceVote(int electionId, byte[] voteHash, byte[] voteEncrypted, ElectionVote previousVote)
    {
        var newVote = new ElectionVote
        {
            ElectionId = electionId,
            IsVerified = false,
            VoteHash = voteHash,
            VotedCandidateEncrypted = voteEncrypted,
            VotesOtp = new VotesOtp
            {
                OtpCode = GenerateRandomOtpCode(10)
            }
        };

        if (previousVote is null)
        {
            newVote = await _votesRepository.AddVote(newVote);
        }
        else
        {
            newVote = await _votesRepository.ReplaceVote(previousVote, newVote);
        }

        return newVote;
    }
    
    private static bool IsUserEligibleVoter(Election election, int userId)
    {
        return election.EligibleVoters.Any(ev => ev.UserId == userId);
    }
    
    private static bool IsElectionOngoing(Election election, int endTimeMargin = 0)
    {
        return election.StartTime < DateTime.Now && election.EndTime.AddMinutes(endTimeMargin) > DateTime.Now;
    }
    
    private static bool IsUserElectionCandidate(IEnumerable<User> candidates, int userId)
    {
        return candidates.Any(c => c.Id == userId);
    }

    private static string GenerateRandomOtpCode(int length)
    {
        const string possibleChars = "0123456789";
        
        string resetCode = RandomNumberGenerator.GetString(possibleChars, length);

        return resetCode;
    }
}