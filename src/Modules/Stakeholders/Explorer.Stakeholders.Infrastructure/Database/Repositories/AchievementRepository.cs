using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly StakeholdersContext _context;

        public AchievementRepository(StakeholdersContext context)
        {
            _context = context;
        }

        public List<Achievement> GetAll()
        {
            return _context.Set<Achievement>().AsNoTracking().ToList();
        }

        public Achievement? GetById(long id)
        {
            return _context.Set<Achievement>()
                .AsNoTracking()
                .FirstOrDefault(a => a.Id == id);
        }

        public Achievement? GetByCode(string code)
        {
            return _context.Set<Achievement>()
                .AsNoTracking()
                .FirstOrDefault(a => a.Code == code);
        }
    }
}
