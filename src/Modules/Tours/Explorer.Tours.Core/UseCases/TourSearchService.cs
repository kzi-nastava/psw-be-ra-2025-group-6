using AutoMapper;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Shared;
using System.Security.Claims;

namespace Explorer.Tours.Core.UseCases
{
    public class TourSearchService : ITourSearchService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public TourSearchService(ITourRepository tourRepository, IMapper mapper)
        {
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<SearchItemDto>> SearchAsync(
            string query,
            ClaimsPrincipal user,
            long personId, string userRole)
        {
            var isAuthor = (userRole == "Author");
            var isAdmin = (userRole == "Administrator");

            return _tourRepository.GetAll()
                .Where(t =>
    (string.IsNullOrWhiteSpace(query) || t.Name.Contains(query)) &&
                    (t.Status == TourStatus.CONFIRMED || isAdmin || (isAuthor && t.AuthorId == personId && t.Status!= TourStatus.SUSPENDED))
                )
                .Select(t => new SearchItemDto
                {
                    Id = t.Id,
                    Title = t.Name,
                    Description = t.Description,
                    Type = SearchEntityType.Tour,
                    Url = $"/tours/{t.Id}",
                    Photo= t.KeyPoints.FirstOrDefault()?.ImagePath?.Replace("{\"url\":\"", "")
    .Replace("\"}", "")
                })
                .ToList();
        }
    }
}
