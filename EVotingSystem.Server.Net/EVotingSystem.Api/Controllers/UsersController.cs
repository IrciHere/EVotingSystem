using EVotingSystem.Contracts.User;
using EVotingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] NewUserDto user)
    {
        UserDto createdUser = await _usersService.CreateUserAsync(user);
        
        return Ok(createdUser);
    }

    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        string userId = User.FindFirst("UserId")?.Value;

        bool wasChanged = await _usersService.ChangePassword(changePasswordDto, userId);
        
        return wasChanged ? Ok() : BadRequest();
    }
}