using AutoMapper;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            long personId)
        {
            var isAuthor = user.IsInRole("Author");
            var isAdmin = user.IsInRole("Admin");

            return _tourRepository.GetAll()
                .Where(t =>
                    t.Name.Contains(query) &&
                    (t.Status == TourStatus.CONFIRMED || isAdmin || (isAuthor && t.AuthorId == personId && t.Status!= TourStatus.SUSPENDED))
                )
                .Select(t => new SearchItemDto
                {
                    Id = t.Id,
                    Title = t.Name,
                    Description = t.Description,
                    Type = SearchEntityType.Tour,
                    Url = $"/tours/{t.Id}"
                })
                .ToList();
        }
    }
}
