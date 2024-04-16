using AutoMapper;
using EVotingSystem.Contracts.Election;
using EVotingSystem.Contracts.User;
using EVotingSystem.Contracts.Vote;
using EVotingSystem.Database.Entities;

namespace EVotingSystem.Api.Mapper;

public class EVotingSystemProfile : Profile  
{
    public EVotingSystemProfile()
    {
        CreateMap<NewUserDto, User>();
        CreateMap<User, UserDto>();

        CreateMap<NewElectionDto, Election>();
        CreateMap<Election, ElectionDto>();

        CreateMap<ElectionVote, VoteDto>()
            .ForMember(dest => dest.VotedCandidateId, opt => opt.MapFrom(src => src.VotedCandidateId ?? 0));
    }
}