using AutoMapper;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<BlogDto, BlogPost>().ReverseMap();
        CreateMap<BlogStatus, BlogStatusDto>().ReverseMap();
    }
}