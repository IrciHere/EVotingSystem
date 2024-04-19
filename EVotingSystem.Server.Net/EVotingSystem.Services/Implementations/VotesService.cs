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

        if (election is null)
        {
            return [];
        }

        if (election.EndTime.AddMinutes(15) > DateTime.Now)
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

        if (election is null)
        {
            return [];
        }

        if (election.EligibleVoters.All(ev => ev.UserId != voterIdNumeric))
        {
            return [];
        }
        
        // get user secret
        byte[] votingSecretEncryptionIV = _encryptionService.GenerateIVArrayFromUserId(user.Id);
        // SHA256 hash of password without salt is used as AES encryption key
        byte[] votingSecretEncryptionKey = _encryptionService
            .HashSHA256(hashCheckDto.Password);
        string votingSecretDecrypted = _encryptionService
            .DecryptSecret(user.UserSecret.VotingSecret, votingSecretEncryptionKey, votingSecretEncryptionIV);
        
        // create hash
        var voteHashObject = new VoteHashModel
        {
            UserId = user.Id,
            ElectionId = election.Id,
            Secret = votingSecretDecrypted
        };

        string voteHashString = JsonSerializer.Serialize(voteHashObject);
        byte[] voteHash = _encryptionService.HashSHA256(voteHashString);

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

        if (election is null)
        {
            return [];
        }

        if (election.StartTime > DateTime.Now || election.EndTime < DateTime.Now)
        {
            return [];
        }
        
        if (election.EligibleVoters.All(ev => ev.UserId != voterIdNumeric))
        {
            return [];
        }
        
        if (election.Candidates.All(c => c.Id != vote.CandidateId))
        {
            return [];
        }
        
        // get user secret
        byte[] votingSecretEncryptionIV = _encryptionService.GenerateIVArrayFromUserId(user.Id);
        // SHA256 hash of password without salt is used as AES encryption key
        byte[] votingSecretEncryptionKey = _encryptionService
            .HashSHA256(vote.VoterPassword);
        string votingSecretDecrypted = _encryptionService
            .DecryptSecret(user.UserSecret.VotingSecret, votingSecretEncryptionKey, votingSecretEncryptionIV);
        
        // create hash
        var voteHashObject = new VoteHashModel
        {
            UserId = user.Id,
            ElectionId = election.Id,
            Secret = votingSecretDecrypted
        };

        string voteHashString = JsonSerializer.Serialize(voteHashObject);
        byte[] voteHash = _encryptionService.HashSHA256(voteHashString);
        
        // set eligible voters to voted
        EligibleVoter voter = election.EligibleVoters.Single(ev => ev.UserId == voterIdNumeric);
        ElectionVote previousVote = null;

        if (!voter.HasVoted)
        {
            voter.HasVoted = true;
        }
        else
        {
            previousVote = await _votesRepository.GetVoteByHash(election.Id, voteHash);

            if (previousVote is null)
            {
                return [];
            }
        }
        
        // encrypt vote
        var voteEncryptObject = new VoteEncryptModel
        {
            CandidateId = vote.CandidateId,
            VoteHash = voteHash
        };
        string voteEncryptText = JsonSerializer.Serialize(voteEncryptObject);

        byte[] ivArray = _encryptionService.GenerateIVArrayFromUserId(vote.ElectionId);
        byte[] voteEncrypted = _encryptionService
            .EncryptSecret(voteEncryptText, election.ElectionSecret.Secret, ivArray);

        var newVote = new ElectionVote
        {
            ElectionId = election.Id,
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
        
        // send SMS
        await _smsService.SendOtpCote(user.PhoneNumber, newVote.VotesOtp.OtpCode);

        return newVote.VoteHash;
    }

    public async Task<byte[]> ValidateVote(ValidateVoteDto validateVoteDto)
    {
        Election election = await _electionRepository.GetElectionById(validateVoteDto.ElectionId);

        if (election is null)
        {
            return [];
        }
        
        if (election.StartTime > DateTime.Now || election.EndTime.AddMinutes(10) < DateTime.Now)
        {
            return [];
        }

        ElectionVote vote = await _votesRepository.GetVoteByHash(election.Id, validateVoteDto.VoteHash, withOtp: true);

        if (vote?.VotesOtp is null)
        {
            return [];
        }

        if (vote.VotesOtp.OtpCode != validateVoteDto.OtpCode)
        {
            return [];
        }

        await _votesRepository.ValidateVote(vote);

        return vote.VoteHash;
    }

    private static string GenerateRandomOtpCode(int length)
    {
        const string possibleChars = "0123456789";
        
        string resetCode = RandomNumberGenerator.GetString(possibleChars, length);

        return resetCode;
    }
}