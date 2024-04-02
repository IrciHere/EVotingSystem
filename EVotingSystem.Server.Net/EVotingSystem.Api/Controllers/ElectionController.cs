using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ElectionController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetElections()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateElection()
    {
        return Ok();
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
}