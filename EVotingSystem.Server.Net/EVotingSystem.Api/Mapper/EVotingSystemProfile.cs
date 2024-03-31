using AutoMapper;
using EVotingSystem.Contracts.User;
using EVotingSystem.Database.Entities;

namespace EVotingSystem.Api.Mapper;

public class EVotingSystemProfile : Profile  
{
    public EVotingSystemProfile()
    {
        CreateMap<NewUserDto, User>();
        CreateMap<User, UserDto>();
    }
}