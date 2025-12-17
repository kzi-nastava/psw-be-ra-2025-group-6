using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class FollowerService : IFollowerService
    {
        private readonly IFollowerRepository followerRepository;
        private readonly IMapper _mapper;

        public FollowerService(IFollowerRepository followerRepository, IMapper mapper)
        {
            this.followerRepository = followerRepository;
            _mapper = mapper;
        }
        public List<FollowerDto> GetAllFollowers(long followerId)
        {
            var result = followerRepository.GetAll(followerId);
            return _mapper.Map<List<FollowerDto>>(result);
        }
    }
}
