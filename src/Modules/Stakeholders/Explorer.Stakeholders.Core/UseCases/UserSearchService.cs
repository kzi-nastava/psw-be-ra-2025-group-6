using AutoMapper;
using Explorer.Stakeholders.API.Services;
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
        private readonly IMapper _mapper;

        public UserSearchService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<SearchItemDto>> SearchAsync(
            string query,
            ClaimsPrincipal user,
            long personId)
        {
            var isAdmin = user.IsInRole("Admin");

            return _userRepository.GetAll()
                .Where(u =>
                    u.Username.Contains(query) &&
                    (isAdmin || u.IsActive==true)
                )
                .Select(u => new SearchItemDto
                {
                    Id = u.Id,
                    Title = u.Username,
                    Description = u.Role.ToString(),
                    Type = SearchEntityType.User,
                    Url = $"/users/{u.Id}"
                })
                .ToList();
        }
    }
}
