using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourProblemRepository : ITourProblemRepository
    {
        private readonly ToursContext _context;

        public TourProblemRepository(ToursContext context)
        {
            _context = context;
        }

        public async Task<List<TourProblem>> GetByTourist(long touristId)
        {
            return await _context.TourProblems
                .Where(p => p.TouristId == touristId)
                .ToListAsync();
        }

        public async Task<TourProblem?> GetById(long id)
        {
            return await _context.TourProblems
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<TourProblem> Create(TourProblem problem)
        {
            _context.TourProblems.Add(problem);
            await _context.SaveChangesAsync();
            return problem;
        }

        public async Task<TourProblem> Update(TourProblem problem)
        {
            _context.TourProblems.Update(problem);
            await _context.SaveChangesAsync();
            return problem;
        }

        public async Task Delete(long id)
        {
            var problem = await GetById(id);
            if (problem != null)
            {
                _context.TourProblems.Remove(problem);
                await _context.SaveChangesAsync();
            }
        }
    }
}