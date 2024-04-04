using EVotingSystem.Contracts.Election;
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

    [HttpPost("assign-voters")]
    public async Task<IActionResult> AssignEligibleVoters()
    {
        return Ok();
    }

    [HttpPost("assign-candidates")]
    public async Task<IActionResult> AssignCandidates()
    {
        return Ok();
    }

    [HttpPatch("finalize-election/{electionId}")]
    public async Task<IActionResult> FinalizeElection(int electionId)
    {
        return Ok();
    }
}