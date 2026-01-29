using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class BlogLocationDto
    {
        public long Id { get; set; }
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string? Region { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}