using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Execution;

public class TourExecutionService : ITourExecutionService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourExecutionRepository _executionRepository;
    private readonly IInternalTourPurchaseTokenService _tokenService;
    private readonly IMapper _mapper;
    private const double ProximityThresholdMeters = 50.0;

    public TourExecutionService(
        ITourRepository tourRepository,
        ITourExecutionRepository executionRepository,
        IInternalTourPurchaseTokenService tokenService,
        IMapper mapper)
    {
        _tourRepository = tourRepository;
        _executionRepository = executionRepository;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public TourExecutionStartResultDto StartExecution(TourExecutionStartDto dto, long touristId)
    {
        var tour = _tourRepository.GetWithKeyPoints(dto.TourId);
        if (tour == null) throw new NotFoundException("Tour not found");

        if (tour.Status != TourStatus.CONFIRMED && tour.Status != TourStatus.ARCHIVED)
            throw new InvalidOperationException("Only published or archived tours can be started.");

        if (!_tokenService.DoesTouristHaveUnusedToken(touristId, dto.TourId))
            throw new InvalidOperationException("Tour must be purchased before starting. Please add it to your cart and checkout.");


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

        KeyPointDto? nextKeyPoint = null;
        List<TrackPointDto>? route = null;

        if (tour.KeyPoints != null && tour.KeyPoints.Any())
        {
            var orderedKeyPoints = tour.KeyPoints.OrderBy(kp => kp.Id).ToList();
            var completedIds = execution.CompletedKeyPoints.Select(ckp => ckp.KeyPointId).ToHashSet();
            
            // Find the NEXT uncompleted key point (not the first one!)
            var nextKp = orderedKeyPoints.FirstOrDefault(kp => !completedIds.Contains(kp.Id));
            
            if (nextKp != null)
            {
                nextKeyPoint = new KeyPointDto
                {
                    Id = nextKp.Id,
                    TourId = nextKp.TourId,
                    Name = nextKp.Name,
                    Description = nextKp.Description,
                    Latitude = nextKp.Latitude,
                    Longitude = nextKp.Longitude,
                    ImagePath = nextKp.ImagePath,
                    Secret = nextKp.Secret
                };

                route = GenerateSimpleRoute(execution.InitialPosition.Latitude, execution.InitialPosition.Longitude, nextKp.Latitude, nextKp.Longitude);
            }
        }

        return new TourExecutionStartResultDto
        {
            TourExecutionId = execution.Id,
            TourId = execution.TourId,
            TouristId = execution.TouristId,
            Status = execution.Status.ToString(),
            StartTime = execution.StartTime,
            InitialPosition = new TrackPointDto { Latitude = execution.InitialPosition.Latitude, Longitude = execution.InitialPosition.Longitude },
            FirstKeyPoint = nextKeyPoint,  // This is actually the NEXT key point now
            RouteToFirstKeyPoint = route
        };
    }

    public ProgressResponseDto CheckProgress(long executionId, TrackPointDto dto, long touristId)
    {
        var execution = _executionRepository.Get(executionId);
        if (execution == null) throw new NotFoundException("Tour execution not found");
        if (execution.TouristId != touristId) throw new ForbiddenException("Not authorized");

        var tour = _tourRepository.GetWithKeyPoints(execution.TourId);
        if (tour.KeyPoints == null || !tour.KeyPoints.Any())
        {
            execution.UpdateLastActivity();
            _executionRepository.Update(execution);
            return new ProgressResponseDto
            {
                KeyPointCompleted = false,
                ProgressPercentage = execution.ProgressPercentage,
                LastActivity = execution.LastActivity,
                AllCompletedKeyPoints = new List<CompletedKeyPointDto>()
            };
        }

        var orderedKeyPoints = tour.KeyPoints.OrderBy(kp => kp.Id).ToList();
        var completedIds = execution.CompletedKeyPoints.Select(ckp => ckp.KeyPointId).ToHashSet();

        KeyPoint? nearbyKeyPoint = null;
        foreach (var kp in orderedKeyPoints.Where(kp => !completedIds.Contains(kp.Id)))
        {
            var distance = DistanceCalculator.CalculateDistance(
                dto.Latitude, dto.Longitude,
                kp.Latitude, kp.Longitude);

            if (distance <= ProximityThresholdMeters)
            {
                nearbyKeyPoint = kp;
                break;
            }
        }

        CompletedKeyPointDto? completedKeyPointDto = null;
        if (nearbyKeyPoint != null)
        {
            execution.CompleteKeyPoint(nearbyKeyPoint.Id, nearbyKeyPoint.Name, nearbyKeyPoint.Secret);
            completedKeyPointDto = new CompletedKeyPointDto
            {
                KeyPointId = nearbyKeyPoint.Id,
                KeyPointName = nearbyKeyPoint.Name,
                UnlockedSecret = nearbyKeyPoint.Secret,
                CompletedAt = execution.CompletedKeyPoints.Last().CompletedAt
            };
        }

        var newProgress = CalculateProgress(orderedKeyPoints, execution.CompletedKeyPoints);
        
        var nextKeyPoint = orderedKeyPoints.FirstOrDefault(kp => !execution.CompletedKeyPoints.Any(ckp => ckp.KeyPointId == kp.Id));
        
        execution.UpdateProgress(newProgress, nextKeyPoint?.Id);
        _executionRepository.Update(execution);

        NextKeyPointDto? nextKeyPointDto = null;
        if (nextKeyPoint != null)
        {
            nextKeyPointDto = new NextKeyPointDto
            {
                KeyPointId = nextKeyPoint.Id,
                Name = nextKeyPoint.Name,
                Description = nextKeyPoint.Description,
                Latitude = nextKeyPoint.Latitude,
                Longitude = nextKeyPoint.Longitude,
                ImagePath = nextKeyPoint.ImagePath
            };
        }

        // Map all completed key points
        var allCompletedKeyPoints = execution.CompletedKeyPoints
            .Select(ckp => new CompletedKeyPointDto
            {
                KeyPointId = ckp.KeyPointId,
                KeyPointName = ckp.KeyPointName,
                UnlockedSecret = ckp.UnlockedSecret,
                CompletedAt = ckp.CompletedAt
            })
            .ToList();

        return new ProgressResponseDto
        {
            KeyPointCompleted = nearbyKeyPoint != null,
            CompletedKeyPoint = completedKeyPointDto,
            ProgressPercentage = execution.ProgressPercentage,
            NextKeyPoint = nextKeyPointDto,
            LastActivity = execution.LastActivity,
            AllCompletedKeyPoints = allCompletedKeyPoints
        };
    }

    public UnlockedSecretsDto GetUnlockedSecrets(long executionId, long touristId)
    {
        var execution = _executionRepository.Get(executionId);
        if (execution == null) throw new NotFoundException("Tour execution not found");
        if (execution.TouristId != touristId) throw new ForbiddenException("Not authorized");

        var secrets = execution.CompletedKeyPoints
            .OrderBy(ckp => ckp.CompletedAt)
            .Select(ckp => new SecretDto
            {
                KeyPointId = ckp.KeyPointId,
                KeyPointName = ckp.KeyPointName,
                Secret = ckp.UnlockedSecret,
                UnlockedAt = ckp.CompletedAt
            })
            .ToList();

        return new UnlockedSecretsDto { Secrets = secrets };
    }

    public TourExecutionResultDto CompleteExecution(long executionId, long touristId)
    {
        var execution = _executionRepository.GetById(executionId);
        if (execution == null) throw new NotFoundException($"Tour execution with id {executionId} not found");
        if (execution.TouristId != touristId) throw new InvalidOperationException("Not authorized to complete this execution");

        execution.Complete();
        _executionRepository.Update(execution);

        // Mark token as used ONLY when tour is completed (not abandoned)
        _tokenService.MarkTokenAsUsed(touristId, execution.TourId);

        return new TourExecutionResultDto
        {
            TourExecutionId = execution.Id,
            TourId = execution.TourId,
            TouristId = execution.TouristId,
            Status = execution.Status.ToString(),
            StartTime = execution.StartTime,
            EndTime = execution.EndTime
        };
    }

    public TourExecutionResultDto AbandonExecution(long executionId, long touristId)
    {
        var execution = _executionRepository.GetById(executionId);
        if (execution == null) throw new NotFoundException($"Tour execution with id {executionId} not found");
        if (execution.TouristId != touristId) throw new InvalidOperationException("Not authorized to abandon this execution");

        execution.Abandon();
        _executionRepository.Update(execution);

        return new TourExecutionResultDto
        {
            TourExecutionId = execution.Id,
            TourId = execution.TourId,
            TouristId = execution.TouristId,
            Status = execution.Status.ToString(),
            StartTime = execution.StartTime,
            EndTime = execution.EndTime
        };
    }

    public List<TourExecutionResultDto> GetExecutedTours(long touristId) 
    {
        var executions = _executionRepository.GetAll(touristId);
        return executions.Select(execution => new TourExecutionResultDto
        {
            TourExecutionId = execution.Id,
            TourId = execution.TourId,
            TouristId = execution.TouristId,
            Status = execution.Status.ToString(),
            StartTime = execution.StartTime,
            EndTime = execution.EndTime,
            LastActivity = execution.LastActivity,
            ProgressPercentage = execution.ProgressPercentage
        }).ToList();
    }

    public List<RecentTourExecutionResultDto> GetRecentExecutedTours(long touristId)
    {
        var executions = _executionRepository.GetAll(touristId);

        return executions.Select(execution =>
        {
            var tour = _tourRepository.GetWithKeyPoints(execution.TourId);

            return new RecentTourExecutionResultDto
            {
                TourExecutionId = execution.Id,
                TourId = execution.TourId,
                TourName = tour.Name,
                TourDescription = tour.Description,
                TouristId = execution.TouristId,
                Status = execution.Status.ToString(),
                StartTime = execution.StartTime,
                EndTime = execution.EndTime,
                LastActivity = execution.LastActivity,
                ProgressPercentage = execution.ProgressPercentage,
                FirstKeyPoint = _mapper.Map<KeyPointDto>(tour.KeyPoints[0])
            };
        }).ToList();
    }

    private double CalculateProgress(List<KeyPoint> orderedKeyPoints, List<CompletedKeyPoint> completedKeyPoints)
    {
        if (orderedKeyPoints.Count == 0) return 0;

        // Simple percentage: (completed / total) * 100
        var completedCount = completedKeyPoints.Count;
        var totalCount = orderedKeyPoints.Count;

        return Math.Round((double)completedCount / totalCount * 100, 2);
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
