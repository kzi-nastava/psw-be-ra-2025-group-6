namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

using Explorer.Stakeholders.Core.Domain;

public interface IProfileRepository
{
    Profile? GetByPersonId(long personId);
    Profile Create(Profile entity);
    Profile Update(Profile entity); 
}