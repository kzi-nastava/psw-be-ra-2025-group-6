using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class PersonDbRepository : IPersonRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Person> _dbSet;

    public PersonDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Person>();
    }

    public Person Create(Person entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }
    public Person GetById(long personId)
    {
        var person = DbContext.People.FirstOrDefault(p => p.Id == personId);
        if (person == null) throw new KeyNotFoundException("Person not found");
        return person;
    }

    public IEnumerable<Person> GetAll()
    {
        return DbContext.People.ToList();
    }

    public void Update(Person person)
    {
        DbContext.People.Update(person);
        DbContext.SaveChanges();
    }
}