using EVotingSystem.Contracts.Election;
using EVotingSystem.Contracts.User;
using EVotingSystem.Models;
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

    [HttpGet("my-available")]
    public async Task<IActionResult> GetMyElections()
    {
        string userId = User.FindFirst("UserId")?.Value;
        
        List<ElectionDto> elections = await _electionService.GetElectionsForUser(userId);

        return Ok(elections);
    }

    [HttpGet("{electionId}/candidates")]
    public async Task<IActionResult> GetElectionCandidates(int electionId)
    {
        List<CandidateDto> candidates = await _electionService.GetElectionCandidates(electionId);

        return candidates.Count > 0 ? Ok(candidates) : NotFound();
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateElection([FromBody] NewElectionDto election)
    {
        ElectionDto newElection = await _electionService.CreateElection(election);
        
        return Ok(newElection);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("{electionId}/assign-voters")]
    public async Task<IActionResult> AssignEligibleVoters(int electionId, [FromBody] List<NewUserDto> users)
    {
        ElectionDto election = await _electionService.AssignEligibleVoters(electionId, users);
        
        return election is not null ? Ok(election) : NotFound();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("{electionId}/assign-candidates")]
    public async Task<IActionResult> AssignCandidates(int electionId, [FromBody] List<NewUserDto> users)
    {
        ElectionDto election = await _electionService.AssignCandidates(electionId, users);
        
        return election is not null ? Ok(election) : NotFound();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPatch("{electionId}/finalize-election")]
    public async Task<IActionResult> FinalizeElection(int electionId)
    {
        ElectionDto election = await _electionService.FinalizeElection(electionId);
        
        return election is not null ? Ok(election) : BadRequest();
    }
}