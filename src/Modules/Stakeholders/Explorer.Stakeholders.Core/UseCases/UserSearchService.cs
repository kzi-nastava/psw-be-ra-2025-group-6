using AutoMapper;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Services;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserSearchService : IUserSearchService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileService _userProfileService;
        private readonly IMapper _mapper;

        public UserSearchService(IUserRepository userRepository, IMapper mapper, IUserProfileService userProfileService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userProfileService = userProfileService;
        }

        public async Task<IReadOnlyCollection<SearchItemDto>> SearchAsync(
            string query,
            ClaimsPrincipal user,
            long personId, string userRole)
        {
 
            var isAdmin = userRole == UserRole.Administrator.ToString();

            var users = _userRepository.GetAll()
            .Where(u =>
                u.Username.Contains(query) &&
                (isAdmin || u.IsActive) &&
                u.Id != personId
                )
                .ToList(); 

            var results = users.Select(u =>
            {
                var profile = _userProfileService.Get(u.Id);

                return new SearchItemDto
                {
                    Id = u.Id,
                    Title = u.Username,
                    Description = u.Role.ToString(),
                    Photo = profile.ProfilePicture,
                    Type = SearchEntityType.User,
                    Url = $"/users/{u.Id}"
                };
            })
            .ToList();

            return results;
        }
    }
}
