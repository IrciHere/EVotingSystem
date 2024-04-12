using EVotingSystem.Contracts.Vote;
using EVotingSystem.Services.Interfaces;

namespace EVotingSystem.Services.Implementations;

public class VotesService : IVotesService
{
    public Task Vote(string voterId, InputVoteDto vote)
    {
        // verify user password
        
        // get user secret
        
        // create hash
        
        // encrypt vote
        
        // create OTP for verification
        
        // save to db
        
        // send SMS

        throw new NotImplementedException();
    }
}