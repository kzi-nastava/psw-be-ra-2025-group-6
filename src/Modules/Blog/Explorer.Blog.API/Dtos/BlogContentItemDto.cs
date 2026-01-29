using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class BlogContentItemDto
    {
        public int Order { get; set; }
        public ContentTypeDto Type { get; set; }
        public string Content { get; set; }
    }

    public enum ContentTypeDto
    {
        Text,
        Image
    }
}
