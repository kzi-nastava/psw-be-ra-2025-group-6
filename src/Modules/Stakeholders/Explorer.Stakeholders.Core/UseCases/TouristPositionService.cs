using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class TouristPositionService : ITouristPositionService
{
    private readonly ITouristPositionRepository _touristPositionRepository;
    private readonly IMapper _mapper;

    public TouristPositionService(ITouristPositionRepository touristPositionRepository, IMapper mapper)
    {
        _touristPositionRepository = touristPositionRepository;
        _mapper = mapper;
    }

    public TouristPositionDto CreateOrUpdate(TouristPositionDto touristPositionDto)
    {
        var existingPosition = _touristPositionRepository.GetByTouristId(touristPositionDto.TouristId);
        if (existingPosition != null)
        {
            existingPosition = _mapper.Map<TouristPosition>(touristPositionDto);
            var result = _touristPositionRepository.CreateOrUpdate(existingPosition);
            return _mapper.Map<TouristPositionDto>(result);
        }
        else
        {
            var position = _mapper.Map<TouristPosition>(touristPositionDto);
            var result = _touristPositionRepository.CreateOrUpdate(position);
            return _mapper.Map<TouristPositionDto>(result);
        }
    }

    public TouristPositionDto? GetByTouristId(long touristId)
    {
        var existing = _touristPositionRepository.GetByTouristId(touristId);
        if (existing == null) return null;
        return _mapper.Map<TouristPositionDto>(existing);
    }
}
