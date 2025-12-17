using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public record TourInfo(long Id, long AuthorId, string Name);

    public interface ITourInfoGateway
    {
        Task<TourInfo?> GetById(long id, CancellationToken cancellationToken = default);
        Task<List<TourInfo>> GetByAuthor(long authorId, CancellationToken cancellationToken = default);
        Task SuspendTour(long id, CancellationToken cancellationToken = default);
    }
}
