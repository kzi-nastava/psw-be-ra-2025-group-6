using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Authoring;

public interface IKeyPointService
{
    List<KeyPointDto> GetPublicKeyPoints();
}
