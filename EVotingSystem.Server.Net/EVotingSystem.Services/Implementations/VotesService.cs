using System.Security.Cryptography;
using System.Text.Json;
using EVotingSystem.Contracts.Vote;
using EVotingSystem.Database.Entities;
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
    public async Task<byte[]> Vote(string voterId, InputVoteDto vote)
    {
        User user = await _usersRepository.GetUserById(int.Parse(voterId), withPassword: true);
        
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

        Election election = await _electionRepository.GetElectionById(vote.ElectionId, withSecret: true); 
        
        // get user secret
        byte[] votingSecretEncryptionIV = _passwordEncryptionService.GenerateIVArrayFromUserId(user.Id);
        // SHA256 hash of password without salt is used as AES encryption key
        byte[] votingSecretEncryptionKey = _passwordEncryptionService
            .HashSHA256(vote.VoterPassword);
        string votingSecretDecrypted = _passwordEncryptionService
            .DecryptSecret(user.UserSecret.VotingSecret, votingSecretEncryptionKey, votingSecretEncryptionIV);
        
        // create hash
        var voteHashObject = new
        {
            UserId = user.Id,
            ElectionId = election.Id,
            Secret = votingSecretDecrypted
        };

        var voteHashString = JsonSerializer.Serialize(voteHashObject);
        var voteHash = _passwordEncryptionService.HashSHA256(voteHashString);
        
        // encrypt vote
        var voteEncryptObject = new
        {
            CandidateId = vote.CandidateId,
            VoteHash = voteHash
        };
        var voteEncryptText = JsonSerializer.Serialize(voteEncryptObject);

        var ivArray = _passwordEncryptionService.GenerateIVArrayFromUserId(vote.ElectionId);
        var voteEncrypted = _passwordEncryptionService
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
        
        // save to db
        newVote = await _votesRepository.AddVote(newVote);
        
        // send SMS
        await _smsService.SendOtpCote(user.PhoneNumber, newVote.VotesOtp.OtpCode);

        return newVote.VoteHash;
    }
    
    private static string GenerateRandomOtpCode(int length)
    {
        const string possibleChars = "0123456789";
        
        string resetCode = RandomNumberGenerator.GetString(possibleChars, length);

        return resetCode;
    }
}