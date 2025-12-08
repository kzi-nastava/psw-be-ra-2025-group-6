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
    }
}