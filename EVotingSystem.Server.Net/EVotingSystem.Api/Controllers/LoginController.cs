using EVotingSystem.Contracts;
using EVotingSystem.Contracts.Login;
using EVotingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[ApiController]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IUsersService _usersService;
    
    public LoginController(ILoginService loginService, IUsersService usersService)
    {
        _loginService = loginService;
        _usersService = usersService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        string token = await _loginService.LoginUser(login);
        
        return string.IsNullOrEmpty(token) ? Unauthorized() : Ok(token);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> RequestResetPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        await _usersService.RequestForgotPassword(forgotPasswordDto.Email);
        
        return Ok();
    }

    [HttpPatch("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        bool wasChanged = await _usersService.ResetPassword(resetPasswordDto);
        
        return wasChanged ? Ok() : Unauthorized();
    }
}