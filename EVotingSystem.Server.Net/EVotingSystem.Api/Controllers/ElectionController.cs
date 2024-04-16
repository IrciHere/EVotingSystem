using EVotingSystem.Contracts.Election;
using EVotingSystem.Contracts.User;
using EVotingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ElectionController : ControllerBase
{
    private readonly IElectionService _electionService;

    public ElectionController(IElectionService electionService)
    {
        _electionService = electionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetElections()
    {
        List<ElectionDto> elections = await _electionService.GetAllElections();
        
        return Ok(elections);
    }

    [HttpPost]
    public async Task<IActionResult> CreateElection([FromBody] NewElectionDto election)
    {
        ElectionDto newElection = await _electionService.CreateElection(election);
        
        return Ok(newElection);
    }

    [HttpPost("{electionId}/assign-voters")]
    public async Task<IActionResult> AssignEligibleVoters(int electionId, [FromBody] List<NewUserDto> users)
    {
        ElectionDto election = await _electionService.AssignEligibleVoters(electionId, users);
        
        return Ok(election);
    }

    [HttpPost("{electionId}/assign-candidates")]
    public async Task<IActionResult> AssignCandidates(int electionId, [FromBody] List<NewUserDto> users)
    {
        ElectionDto election = await _electionService.AssignCandidates(electionId, users);
        
        return Ok(election);
    }

    [HttpPatch("{electionId}/finalize-election")]
    public async Task<IActionResult> FinalizeElection(int electionId)
    {
        ElectionDto election = await _electionService.FinalizeElection(electionId);
        
        return election is not null ? Ok(election) : BadRequest();
    }
}