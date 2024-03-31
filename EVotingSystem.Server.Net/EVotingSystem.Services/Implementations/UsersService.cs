using AutoMapper;
using EVotingSystem.Contracts.User;
using EVotingSystem.Database.Entities;
using EVotingSystem.Repositories.Implementations;
using EVotingSystem.Services.Interfaces;

namespace EVotingSystem.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;

    public UsersService(IUsersRepository usersRepository, IMapper mapper)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> CreateUserAsync(NewUserDto user)
    {
        var newUserEntity = _mapper.Map<User>(user);

        newUserEntity = await _usersRepository.CreateUser(newUserEntity);

        var newUser = _mapper.Map<UserDto>(newUserEntity);

        return newUser;
    }
}