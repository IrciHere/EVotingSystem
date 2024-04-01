using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Login()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> RequestResetPassword()
    {
        return Ok();
    }

    [HttpPatch]
    public async Task<IActionResult> ResetPassword()
    {
        return Ok();
    }
}