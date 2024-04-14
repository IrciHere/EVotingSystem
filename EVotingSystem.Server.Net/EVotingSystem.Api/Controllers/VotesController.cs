﻿using EVotingSystem.Contracts.Vote;
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

    [HttpGet]
    public async Task<IActionResult> GetAllElectionVotes()
    {
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Vote([FromBody] InputVoteDto vote)
    {
        string userId = User.FindFirst("UserId")?.Value;

        var voteHash = await _votesService.Vote(userId, vote);
        
        return voteHash.Length == 0 ? BadRequest() : Ok(voteHash);
    }

    [HttpPost]
    public async Task<IActionResult> VerifyVote()
    {
        return Ok();
    }
}