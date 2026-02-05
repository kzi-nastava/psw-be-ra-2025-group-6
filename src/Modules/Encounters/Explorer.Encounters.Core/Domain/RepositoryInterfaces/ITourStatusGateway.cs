using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface ITourStatusGateway
    {
        Task<string?> GetTourStatusByKeyPointId(long keyPointId);
    }
}
