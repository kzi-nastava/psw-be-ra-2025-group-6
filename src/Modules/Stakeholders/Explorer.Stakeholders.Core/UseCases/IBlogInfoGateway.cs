using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases;

public record BlogInfo(long Id, long AuthorId, string Title);

public interface IBlogInfoGateway
{
    Task<BlogInfo?> GetById(long id, CancellationToken cancellationToken = default);
    Task<List<BlogInfo>> GetByAuthor(long authorId, CancellationToken cancellationToken = default);
}
