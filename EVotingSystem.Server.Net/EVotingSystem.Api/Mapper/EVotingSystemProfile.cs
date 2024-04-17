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
        CreateMap<User, CandidateDto>();

        CreateMap<NewElectionDto, Election>();
        CreateMap<Election, ElectionDto>();

        CreateMap<ElectionVote, VoteDto>()
            .ForMember(dest => dest.VotedCandidateId, opt => opt.MapFrom(src => src.VotedCandidateId ?? 0));

        CreateMap<ElectionResult, ElectionResultDto>()
            .ForMember(dest => dest.CandidateId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => src.User.Name));
    }
}