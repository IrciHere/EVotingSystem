using EVotingSystem.Contracts.User;
using EVotingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem.Api.Controllers;

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
        NewUserDto createdUser = await _usersService.CreateUserAsync(user);
        
        return Ok(createdUser);
    }
}