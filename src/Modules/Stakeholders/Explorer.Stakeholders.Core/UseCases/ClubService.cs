using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;

        public ClubService(IClubRepository clubRepository, IMapper mapper)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
        }

        public ClubDto Create(ClubDto clubDto)
        {
            var clubDomain = _mapper.Map<Club>(clubDto);
            var result = _clubRepository.Create(clubDomain);
            return _mapper.Map<ClubDto>(result);
        }

        public ClubDto Update(ClubDto clubDto)
        {
            var clubDomain = _mapper.Map<Club>(clubDto);
            var result = _clubRepository.Update(clubDomain);
            return _mapper.Map<ClubDto>(result);
        }

        public void Delete(long id)
        {
            _clubRepository.Delete(id);
        }

        public ClubDto Get(long id)
        {
            var result = _clubRepository.Get(id);
            return _mapper.Map<ClubDto>(result);
        }

        public List<ClubDto> GetAll()
        {
            var result = _clubRepository.GetAll();
            return _mapper.Map<List<ClubDto>>(result);
        }
    }
}