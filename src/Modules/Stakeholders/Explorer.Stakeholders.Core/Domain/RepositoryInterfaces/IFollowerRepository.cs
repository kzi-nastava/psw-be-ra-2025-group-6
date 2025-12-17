namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IFollowerRepository
    {
        List<Follower> GetAll(long UserId);
    }

}
