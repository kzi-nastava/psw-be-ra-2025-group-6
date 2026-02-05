using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IAchievementService
    {
        List<AchievementDto> GetAll();
        AchievementDto GetById(long id);
    }
}
