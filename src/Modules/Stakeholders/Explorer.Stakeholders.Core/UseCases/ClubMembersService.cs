using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubMemberService : IClubMemberService
    {
        private readonly IClubMemberRepository _clubMemberRepository;
        private readonly IMapper _mapper;

        public ClubMemberService(IClubMemberRepository clubMemberRepository, IMapper mapper)
        {
            _clubMemberRepository = clubMemberRepository;
            _mapper = mapper;
        }
        public List<ClubMemberDto> GetAllClubMembers(long clubId)
        {
            var result = _clubMemberRepository.GetAll(clubId);
            return _mapper.Map<List<ClubMemberDto>>(result);
        }
    }
}