using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class JournalDbRepository : IJournalRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<Journal> _journals;

        public JournalDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _journals = dbContext.Journals;
        }

        public async Task<Journal> Save(Journal journal)
        {
            if (journal.Id == 0)
            {
                await _journals.AddAsync(journal);
            }
            else
            {
                _journals.Update(journal);
            }
            await _dbContext.SaveChangesAsync();
            return journal;
        }

        public async Task<Journal?> GetById(long journalId)
        {
            var journal = await _journals.FindAsync(journalId);
            if (journal == null) throw new NotFoundException($"Journal not found: {journalId}");
            return journal;
        }

        public async Task<List<Journal>> GetAllByTouristId(long touristId)
        {
            return await _journals
                .Where(j => j.TouristId == touristId)
                .ToListAsync();
        }

        public async Task Delete(long journalId)
        {
            var journalToDelete = await _journals.FindAsync(journalId);
            if (journalToDelete != null)
            {
                _journals.Remove(journalToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
