using System.Security.Cryptography;
using System.Text.Json;
using EVotingSystem.Contracts.Vote;
using EVotingSystem.Database.Entities;
using EVotingSystem.Models;
using EVotingSystem.Repositories.Interfaces;
using EVotingSystem.Services.Interfaces;
using EVotingSystem.Services.Interfaces.Helpers;

namespace EVotingSystem.Services.Implementations;

public class VotesService : IVotesService
{
    private readonly IElectionRepository _electionRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IVotesRepository _votesRepository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly ISmsService _smsService;

    public VotesService(IElectionRepository electionRepository, IUsersRepository usersRepository, IVotesRepository votesRepository, IPasswordEncryptionService passwordEncryptionService, ISmsService smsService)
    {
        _electionRepository = electionRepository;
        _usersRepository = usersRepository;
        _votesRepository = votesRepository;
        _passwordEncryptionService = passwordEncryptionService;
        _smsService = smsService;
    }

    public async Task<byte[]> Vote(string voterId, InputVoteDto vote)
    {
        int voterIdNumeric = int.Parse(voterId);
        User user = await _usersRepository.GetUserById(voterIdNumeric, withPassword: true);
        
        if (user is null)
        {
            return []; 
        }
        
        // verify user password
        byte[] hashedPassword =
            _passwordEncryptionService.HashSHA256WithSalt(vote.VoterPassword, user.UserSecret.PasswordSalt);
        
        if (!hashedPassword.SequenceEqual(user.UserPassword.PasswordHash))
        {
            return [];
        }

        Election election = await _electionRepository
            .GetElectionById(vote.ElectionId, withSecret: true, withCandidates: true, withVoters: true);

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
        byte[] votingSecretEncryptionIV = _passwordEncryptionService.GenerateIVArrayFromUserId(user.Id);
        // SHA256 hash of password without salt is used as AES encryption key
        byte[] votingSecretEncryptionKey = _passwordEncryptionService
            .HashSHA256(vote.VoterPassword);
        string votingSecretDecrypted = _passwordEncryptionService
            .DecryptSecret(user.UserSecret.VotingSecret, votingSecretEncryptionKey, votingSecretEncryptionIV);
        
        // create hash
        var voteHashObject = new VoteHashModel
        {
            UserId = user.Id,
            ElectionId = election.Id,
            Secret = votingSecretDecrypted
        };

        string voteHashString = JsonSerializer.Serialize(voteHashObject);
        byte[] voteHash = _passwordEncryptionService.HashSHA256(voteHashString);
        
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

        byte[] ivArray = _passwordEncryptionService.GenerateIVArrayFromUserId(vote.ElectionId);
        byte[] voteEncrypted = _passwordEncryptionService
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