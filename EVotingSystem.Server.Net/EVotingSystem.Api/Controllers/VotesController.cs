using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VotesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllElectionVotes()
    {
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Vote()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyVote()
    {
        return Ok();
    }
}