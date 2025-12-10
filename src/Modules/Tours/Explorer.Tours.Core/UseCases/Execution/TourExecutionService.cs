using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.UseCases.Execution;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourExecutionRepository _executionRepository;
    private readonly IMapper _mapper;

    public TourExecutionService(ITourRepository tourRepository, ITourExecutionRepository executionRepository, IMapper mapper)
    {
        _tourRepository = tourRepository;
        _executionRepository = executionRepository;
        _mapper = mapper;
    }

    public TourExecutionStartResultDto StartExecution(TourExecutionStartDto dto, long touristId)
    {
        var tour = _tourRepository.GetWithKeyPoints(dto.TourId);
        if (tour == null) throw new NotFoundException("Tour not found");

        if (tour.Status != TourStatus.CONFIRMED && tour.Status != TourStatus.ARCHIVED)
            throw new InvalidOperationException("Only published or archived tours can be started.");

        var initial = new TrackPoint(dto.Latitude, dto.Longitude);

        var execution = new TourExecution(dto.TourId, touristId, initial);

        var created = _executionRepository.Create(execution);

        KeyPointDto? firstKeyPoint = null;
        List<TrackPointDto>? route = null;

        if (tour.KeyPoints != null && tour.KeyPoints.Any())
        {
            var firstKp = tour.KeyPoints.OrderBy(kp => kp.Id).First();
            firstKeyPoint = new KeyPointDto
            {
                Id = firstKp.Id,
                TourId = firstKp.TourId,
                Name = firstKp.Name,
                Description = firstKp.Description,
                Latitude = firstKp.Latitude,
                Longitude = firstKp.Longitude,
                ImagePath = firstKp.ImagePath,
                Secret = firstKp.Secret
            };

            route = GenerateSimpleRoute(dto.Latitude, dto.Longitude, firstKp.Latitude, firstKp.Longitude);
        }

        return new TourExecutionStartResultDto
        {
            TourExecutionId = created.Id,
            TourId = created.TourId,
            TouristId = created.TouristId,
            Status = created.Status.ToString(),
            StartTime = created.StartTime,
            InitialPosition = new TrackPointDto { Latitude = created.InitialPosition.Latitude, Longitude = created.InitialPosition.Longitude },
            FirstKeyPoint = firstKeyPoint,
            RouteToFirstKeyPoint = route
        };
    }

    public TourExecutionStartResultDto? GetActiveExecution(long touristId, long? tourId = null)
    {
        var execution = _executionRepository.GetActiveForTourist(touristId, tourId);
        if (execution == null) return null;

        var tour = _tourRepository.GetWithKeyPoints(execution.TourId);

        KeyPointDto? firstKeyPoint = null;
        List<TrackPointDto>? route = null;

        if (tour.KeyPoints != null && tour.KeyPoints.Any())
        {
            var firstKp = tour.KeyPoints.OrderBy(kp => kp.Id).First();
            firstKeyPoint = new KeyPointDto
            {
                Id = firstKp.Id,
                TourId = firstKp.TourId,
                Name = firstKp.Name,
                Description = firstKp.Description,
                Latitude = firstKp.Latitude,
                Longitude = firstKp.Longitude,
                ImagePath = firstKp.ImagePath,
                Secret = firstKp.Secret
            };

            route = GenerateSimpleRoute(execution.InitialPosition.Latitude, execution.InitialPosition.Longitude, firstKp.Latitude, firstKp.Longitude);
        }

        return new TourExecutionStartResultDto
        {
            TourExecutionId = execution.Id,
            TourId = execution.TourId,
            TouristId = execution.TouristId,
            Status = execution.Status.ToString(),
            StartTime = execution.StartTime,
            InitialPosition = new TrackPointDto { Latitude = execution.InitialPosition.Latitude, Longitude = execution.InitialPosition.Longitude },
            FirstKeyPoint = firstKeyPoint,
            RouteToFirstKeyPoint = route
        };
    }

    public TourExecutionResultDto CompleteExecution(long executionId, long touristId)
    {
        var execution = _executionRepository.GetById(executionId);
        if (execution == null) throw new NotFoundException("Execution not found");

        if (execution.TouristId != touristId)
            throw new InvalidOperationException("Cannot complete execution that does not belong to tourist");

        execution.Complete();
        _executionRepository.Update(execution);

        return new TourExecutionResultDto
        {
            TourExecutionId = execution.Id,
            TourId = execution.TourId,
            TouristId = execution.TouristId,
            Status = execution.Status.ToString(),
            StartTime = execution.StartTime,
            EndTime = execution.EndTime,
            LastActivity = execution.LastActivity
        };
    }

    public TourExecutionResultDto AbandonExecution(long executionId, long touristId)
    {
        var execution = _executionRepository.GetById(executionId);
        if (execution == null) throw new NotFoundException("Execution not found");

        if (execution.TouristId != touristId)
            throw new InvalidOperationException("Cannot abandon execution that does not belong to tourist");

        execution.Abandon();
        _executionRepository.Update(execution);

        return new TourExecutionResultDto
        {
            TourExecutionId = execution.Id,
            TourId = execution.TourId,
            TouristId = execution.TouristId,
            Status = execution.Status.ToString(),
            StartTime = execution.StartTime,
            EndTime = execution.EndTime,
            LastActivity = execution.LastActivity
        };
    }

    private List<TrackPointDto> GenerateSimpleRoute(double startLat, double startLng, double endLat, double endLng)
    {
        return new List<TrackPointDto>
        {
            new TrackPointDto { Latitude = startLat, Longitude = startLng },
            new TrackPointDto { Latitude = endLat, Longitude = endLng }
        };
    }
}
