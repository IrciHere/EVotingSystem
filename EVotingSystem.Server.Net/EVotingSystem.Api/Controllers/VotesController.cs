using EVotingSystem.Contracts.Vote;
using EVotingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class VotesController : ControllerBase
{
    private readonly IVotesService _votesService;
    
    public VotesController(IVotesService votesService)
    {
        _votesService = votesService;
    }

    [HttpGet("election/{electionId}")]
    public async Task<IActionResult> GetAllElectionVotes(int electionId)
    {
        List<VoteDto> votes = await _votesService.GetAllVotesForElection(electionId);
        
        return votes.Count > 0 ? Ok(votes) : BadRequest();
    }
    
    [HttpPost("my-vote-hash")]
    public async Task<IActionResult> GetMyVoteHashForElection([FromBody] HashCheckDto hashCheckDto)
    {
        string userId = User.FindFirst("UserId")?.Value;
        
        byte[] myHash = await _votesService.GetMyVoteHashForElection(userId, hashCheckDto);
        
        return Ok(myHash);
    }
    
    [HttpPut]
    public async Task<IActionResult> Vote([FromBody] InputVoteDto vote)
    {
        string userId = User.FindFirst("UserId")?.Value;

        byte[] voteHash = await _votesService.Vote(userId, vote);
        
        return voteHash.Length == 0 ? BadRequest() : Ok(voteHash);
    }

    [HttpPost("validate-vote")]
    public async Task<IActionResult> ValidateVote([FromBody] ValidateVoteDto validateVote)
    {
        byte[] voteHash = await _votesService.ValidateVote(validateVote);
        
        return voteHash.Length == 0 ? BadRequest() : Ok();
    }
}