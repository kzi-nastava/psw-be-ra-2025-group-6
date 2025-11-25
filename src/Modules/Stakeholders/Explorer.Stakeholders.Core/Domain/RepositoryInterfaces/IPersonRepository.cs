namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IPersonRepository
{
    Person Create(Person person);
    Person GetById(long personId);
    IEnumerable<Person> GetAll();
    void Update(Person person);
}