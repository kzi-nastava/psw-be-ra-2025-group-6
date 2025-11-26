using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class FacilityDbRepository : IFacilityRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<Facility> _dbSet;

        public FacilityDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Facility>();
        }
        public PagedResult<Facility> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }
        public Facility Get(long Id)
        {
            var entity = _dbSet.Find(Id);
            if (entity == null) throw new NotFoundException("Not found: " + Id);
            return entity;
        }
        public Facility Create(Facility f)
        {
            _dbSet.Add(f);
            DbContext.SaveChanges();
            return f;
        }
        public Facility Update(Facility f)
        {
            try
            {
                DbContext.Update(f);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return f;

        }
        public void Delete(long Id)
        {
            var entity = Get(Id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public Facility GetUntracked(long id)
        {
            var entity = _dbSet.AsNoTracking().FirstOrDefault(f => f.Id == id);
            if (entity == null) throw new NotFoundException("Not found: " + id);
            return entity;
        }
    }
}
