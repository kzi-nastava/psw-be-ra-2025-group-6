using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<BlogPost, BlogDto>().ReverseMap()
            .ForMember(d => d.Comments, opt => opt.MapFrom(s => s.Comments));
        CreateMap<BlogDto, BlogPost>();
        CreateMap<Comment, CommentDto>().ReverseMap();
        CreateMap<BlogDto, BlogPost>().ReverseMap();
        CreateMap<BlogStatus, BlogStatusDto>().ReverseMap();
        //CreateMap<BlogVoteDto, BlogVote>().ReverseMap();
        CreateMap<VoteType, VoteTypeDto>().ConvertUsing(src => (VoteTypeDto)(int)src);
        CreateMap<VoteTypeDto, VoteType>().ConvertUsing(src => (VoteType)(int)src);
        CreateMap<BlogVote, BlogVoteDto>().ForMember(dest => dest.Type, opt => opt.MapFrom(src => (VoteTypeDto)(int)src.Type));
        CreateMap<BlogVoteDto, BlogVote>().ForMember(dest => dest.Type, opt => opt.MapFrom(src => (VoteType)(int)src.Type));
        CreateMap<BlogQualityStatus, BlogQualityStatusDto>().ReverseMap();
        CreateMap<BlogLocation, BlogLocationDto>().ReverseMap();
        CreateMap<BlogContentItem, BlogContentItemDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => (ContentTypeDto)s.Type));
        CreateMap<BlogContentItemDto, BlogContentItem>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => (ContentType)s.Type));

    }
}