using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ResultsController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSummarizedResults([FromRoute] int id)
    {
        return Ok();
    }
}