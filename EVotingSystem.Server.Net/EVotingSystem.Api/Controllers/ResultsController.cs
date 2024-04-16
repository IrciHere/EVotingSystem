using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ResultsController : ControllerBase
{
    [HttpGet("{electionId}")]
    public async Task<IActionResult> GetSummarizedResults(int electionId)
    {
        return Ok();
    }
}