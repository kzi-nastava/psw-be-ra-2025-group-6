using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TouristEquipmentDbRepository : ITouristEquipmentRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TouristEquipment> _dbSet;

    public TouristEquipmentDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TouristEquipment>();
    }

    public PagedResult<TouristEquipment> GetOwned(long personId, int page, int pageSize)
    {
        var task = _dbSet.Where(pe => pe.PersonId == personId).GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public TouristEquipment Create(TouristEquipment entity)
    {
        // Prevent assigning equipment that's already owned by another person
        var existing = _dbSet.FirstOrDefault(te => te.EquipmentId == entity.EquipmentId);
        if (existing != null)
        {
            // If it's already owned by same person, return existing; otherwise throw
            if (existing.PersonId == entity.PersonId) return existing;
            throw new EntityValidationException("Equipment is already assigned to another person");
        }

        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public void Delete(long personId, long equipmentId)
    {
        var entity = _dbSet.FirstOrDefault(pe => pe.PersonId == personId && pe.EquipmentId == equipmentId);
        if (entity == null) throw new NotFoundException("Not found");
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public TouristEquipment? GetByEquipmentId(long equipmentId)
    {
        return _dbSet.FirstOrDefault(te => te.EquipmentId == equipmentId);
    }

    public IEnumerable<long> GetAssignedEquipmentIds()
    {
        return _dbSet.Select(te => te.EquipmentId).Distinct().ToList();
    }
}
