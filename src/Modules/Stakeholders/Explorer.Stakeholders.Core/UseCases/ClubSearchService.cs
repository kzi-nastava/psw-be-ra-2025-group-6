using AutoMapper;
using Explorer.Stakeholders.Core.Domain;
using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Core.UseCases;

        public class ClubSearchService : IClubSearchService
{
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;
        public ClubSearchService(IClubRepository clubRepository, IMapper mapper)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<SearchItemDto>> SearchAsync(
            string query,
            ClaimsPrincipal user, long personId, string userRole)
        {

            var isAuthor = userRole == UserRole.Author.ToString();
            var isAdmin = userRole == UserRole.Administrator.ToString();

            var clubs = _clubRepository.GetAll()
                .Where(c =>
    (string.IsNullOrWhiteSpace(query)) || c.Name.Contains(query));
        var clubList = clubs
        .Select(c =>
        {
        var image = c.ImageUris.FirstOrDefault();

        return new SearchItemDto
        {
            Id = c.Id,
            Title = c.Name,
            Description = c.Description,
            Type = SearchEntityType.Club,
            Url = $"/clubs/{c.Id}",
            Photo = Uri.TryCreate(image, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp)
            ? image
            : null
            };
            })
            .ToList();

        return clubList;
        }
    }

