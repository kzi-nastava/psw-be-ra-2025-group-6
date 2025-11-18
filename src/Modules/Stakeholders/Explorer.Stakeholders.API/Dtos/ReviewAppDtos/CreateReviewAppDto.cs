using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos.ReviewAppDtos
{
    public class CreateReviewAppDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
