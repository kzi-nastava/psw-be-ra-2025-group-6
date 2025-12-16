using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IMapper _mapper;

    public TourService(ITourRepository repository, IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _tourRepository = repository;
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

    public List<TourDto> GetAll() {
        var result = _tourRepository.GetAll();

        var items=_mapper.Map<List<TourDto>>(result);
        return new List<TourDto>(items);
    }

    public PagedResult<TourDto> GetPaged(int page, int pageSize)
    {
        var result = _tourRepository.GetPaged(page, pageSize);

        var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
        return new PagedResult<TourDto>(items, result.TotalCount);
    }
    public TourDto Get(long id)
    {
        var result = _tourRepository.Get(id);
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Create(TourDto entity)
    {
        var result = _tourRepository.Create(_mapper.Map<Tour>(entity));
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Update(TourDto entity)
    {
        var result = _tourRepository.Update(_mapper.Map<Tour>(entity));
        return _mapper.Map<TourDto>(result);
    }

    public void Delete(long id)
    {
        TourDto item = Get(id);
        if (item.Status != TourStatusDto.DRAFT) {
            throw new InvalidOperationException("Only tours in Draft status can be deleted.");
        }
        else
            _tourRepository.Delete(id);
    }
    public TourDto Archive(long tourId, long authorId)
    {
        var tour = _tourRepository.Get(tourId);
        tour.Archive(authorId);
        _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(tour);
    }

    public TourDto Activate(long tourId, long authorId)
    {
        var tour = _tourRepository.Get(tourId);
        tour.Activate(authorId);
        _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(tour);
    }


    public void AddEquipmentToTour(long tourId, long equipmentId)
    {
        var tour = _tourRepository.Get(tourId);
        var equipment = _equipmentRepository.Get(equipmentId);
        if (tour == null)
            throw new KeyNotFoundException($"Tour with id {tourId} not found.");
        if (equipment == null)
            throw new KeyNotFoundException($"Equipment with id {equipmentId} not found.");

        tour.AddEquipment(equipment);

        _tourRepository.Update(tour);
    }

    public void RemoveEquipmentFromTour(long tourId, long equipmentId)
    {
        var tour = _tourRepository.Get(tourId);
        var equipment = _equipmentRepository.Get(equipmentId);
        if (tour == null)
            throw new KeyNotFoundException($"Tour with id {tourId} not found.");
        if (equipment == null)
            throw new KeyNotFoundException($"Equipment with id {equipmentId} not found.");

        tour.RemoveEquipment(equipment);

        _tourRepository.Update(tour);
    }
    public TourDto AddKeyPoint(long tourId, KeyPointDto keyPoint)
    {
        var tour = _tourRepository.Get(tourId);

        keyPoint.TourId = tourId;
        var domainKeyPoint = _mapper.Map<KeyPoint>(keyPoint);

        tour.AddKeyPoint(domainKeyPoint);

        var result = _tourRepository.Update(tour);

        return _mapper.Map<TourDto>(result);
    }
    public TourDto UpdateTourDistance(long tourId, double distance)
    {
        var tour = _tourRepository.Get(tourId);

        tour.SetDistance(distance);

        var result = _tourRepository.Update(tour);

        return _mapper.Map<TourDto>(result);
    }

    public TourDto UpdateDuration(long tourId, List<TourDurationDto> durations)
    {
        var tour = _tourRepository.Get(tourId);

        foreach (var dto in durations)
        {
            var duration=_mapper.Map<TourDuration>(dto);
            var existing = tour.Duration.FirstOrDefault(d => d.TravelType == duration.TravelType);
            if (existing != null)
            {
                existing.UpdateMinutes(dto.Minutes);
            }
            else
            {
                tour.SetDuration(duration); 
            }
        }

        _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(tour);
    }
    private TouristTourDto MapToTouristView(Tour tour)
    {
        return new TouristTourDto
        {
            Name = tour.Name,
            FirstKeyPoint = _mapper.Map<KeyPointDto>(tour.GetFirstKeyPoint()),
            Difficulty = (TourDifficultyDto)tour.Difficulty,
            Price = tour.Price,
            Tags = tour.Tags,
            DistanceInKm = tour.DistanceInKm,
            Duration = _mapper.Map<List<TourDurationDto>>(tour.Duration),
            Description = tour.Description,
        };
    }

    public TourDto Publish(long tourId, long authorId)
    {
        var tour = _tourRepository.Get(tourId);

        tour.Publish(authorId);

        _tourRepository.Update(tour);

        return _mapper.Map<TourDto>(tour);
    }
}
