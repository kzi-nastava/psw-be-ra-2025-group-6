using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class KeyPointService : IKeyPointService
{
    private readonly IKeyPointRepository _keyPointRepository;
    private readonly IMapper _mapper;

    public KeyPointService(IKeyPointRepository keyPointRepository, IMapper mapper)
    {
        _keyPointRepository = keyPointRepository;
        _mapper = mapper;
    }

    public List<KeyPointDto> GetPublicKeyPoints()
    {
        var keyPoints = _keyPointRepository.GetPublicKeyPoints();
        return keyPoints.Select(_mapper.Map<KeyPointDto>).ToList();
    }
}
