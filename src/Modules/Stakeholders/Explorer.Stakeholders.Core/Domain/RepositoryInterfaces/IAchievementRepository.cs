using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IAchievementRepository
    {
        List<Achievement> GetAll();
        Achievement? GetById(long id);
        Achievement? GetByCode(string code);
    }
}
