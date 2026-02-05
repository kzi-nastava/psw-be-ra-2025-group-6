using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases
{
    public class TourCheckpointPlanService : ITourCheckpointPlanService
    {
        private readonly IMapper _mapper;
        private readonly ITourCheckpointPlanRepository _repository;
        private readonly ITourPlannerRepository _tourPlannerRepository;
        private readonly ITourRepository _tourRepository;

        public TourCheckpointPlanService(
            IMapper mapper,
            ITourCheckpointPlanRepository repository,
            ITourPlannerRepository tourPlannerRepository,
            ITourRepository tourRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _tourPlannerRepository = tourPlannerRepository;
            _tourRepository = tourRepository;
        }

        public TourCheckpointPlanDto Create(long userId, TourCheckpointPlanCreateDto dto)
        {
            var planner = _tourPlannerRepository.GetById(dto.PlannerItemId);
            EnsureOwner(userId, planner.UserId);
            EnsurePlannedAtInRange(dto.PlannedAt, planner);
            EnsureKeyPointInTour(dto.KeyPointId, planner.TourId);

            var plan = new TourCheckpointPlan(userId, planner.Id, dto.KeyPointId, dto.PlannedAt);
            var created = _repository.Create(plan);
            return _mapper.Map<TourCheckpointPlanDto>(created);
        }

        public TourCheckpointPlanDto Update(long id, long userId, TourCheckpointPlanUpdateDto dto)
        {
            var existing = _repository.GetById(id);
            EnsureOwner(userId, existing.UserId);

            var planner = _tourPlannerRepository.GetById(existing.PlannerItemId);
            EnsureOwner(userId, planner.UserId);
            EnsurePlannedAtInRange(dto.PlannedAt, planner);

            existing.UpdatePlannedAt(dto.PlannedAt);
            _repository.Update(existing);
            return _mapper.Map<TourCheckpointPlanDto>(existing);
        }

        public void Delete(long id, long userId)
        {
            var existing = _repository.GetById(id);
            EnsureOwner(userId, existing.UserId);
            _repository.Delete(id);
        }

        public TourCheckpointPlanDto GetById(long id, long userId)
        {
            var existing = _repository.GetById(id);
            EnsureOwner(userId, existing.UserId);
            return _mapper.Map<TourCheckpointPlanDto>(existing);
        }

        public List<TourCheckpointPlanDto> GetByPlannerItemId(long plannerItemId, long userId)
        {
            var planner = _tourPlannerRepository.GetById(plannerItemId);
            EnsureOwner(userId, planner.UserId);

            var plans = _repository.GetByPlannerItemId(plannerItemId);
            return plans.Select(_mapper.Map<TourCheckpointPlanDto>).ToList();
        }

        private static void EnsureOwner(long userId, long ownerId)
        {
            if (userId != ownerId)
            {
                throw new UnauthorizedAccessException("You can not access someone else's planner data.");
            }
        }

        private static void EnsurePlannedAtInRange(DateTime plannedAt, TourPlanner planner)
        {
            if (plannedAt < planner.StartDate || plannedAt > planner.EndDate)
            {
                throw new EntityValidationException("Planned time must be within the tour plan range.");
            }
        }

        private void EnsureKeyPointInTour(long keyPointId, long tourId)
        {
            var tour = _tourRepository.GetWithKeyPoints(tourId);
            if (!tour.KeyPoints.Any(kp => kp.Id == keyPointId))
            {
                throw new EntityValidationException("Key point does not belong to the tour.");
            }
        }
    }
}
