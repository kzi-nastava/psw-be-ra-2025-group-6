using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<BlogDto, BlogPost>().ReverseMap();
        CreateMap<BlogStatus, BlogStatusDto>().ReverseMap();
        //CreateMap<BlogVoteDto, BlogVote>().ReverseMap();
        CreateMap<VoteType, VoteTypeDto>().ConvertUsing(src => (VoteTypeDto)(int)src);
        CreateMap<VoteTypeDto, VoteType>().ConvertUsing(src => (VoteType)(int)src);
        CreateMap<BlogVote, BlogVoteDto>().ForMember(dest => dest.Type, opt => opt.MapFrom(src => (VoteTypeDto)(int)src.Type));
        CreateMap<BlogVoteDto, BlogVote>().ForMember(dest => dest.Type, opt => opt.MapFrom(src => (VoteType)(int)src.Type));
    }
}