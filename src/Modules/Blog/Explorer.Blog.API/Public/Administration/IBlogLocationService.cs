using Explorer.Blog.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Public.Administration
{
    public interface IBlogLocationService
    {
        BlogLocationDto CreateOrGet(BlogLocationDto dto);
        BlogLocationDto GetById(long id);
        List<BlogLocationDto> GetAll();
    }
}
