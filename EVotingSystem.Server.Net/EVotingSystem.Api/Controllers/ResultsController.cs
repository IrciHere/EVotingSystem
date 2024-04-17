using EVotingSystem.Contracts.Election;
using EVotingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ResultsController : ControllerBase
{
    private readonly IElectionService _electionService;

    public ResultsController(IElectionService electionService)
    {
        _electionService = electionService;
    }

    [HttpGet("{electionId}")]
    public async Task<IActionResult> GetSummarizedResults(int electionId)
    {
        List<ElectionResultDto> results = await _electionService.GetElectionResults(electionId);
        
        return results.Count > 0 ? Ok(results) : NotFound();
    }
}