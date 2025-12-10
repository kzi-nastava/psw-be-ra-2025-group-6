using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public
{
     public interface ITouristViewService
    {
        List<TouristTourDto> GetPublishedTours();

    }
}
