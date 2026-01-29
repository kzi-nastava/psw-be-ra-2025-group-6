using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Shared;
using Shared.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases
{
    public class TourPlannerService : ITourPlannerService
    {
        private readonly IMapper _mapper;
        private readonly ITourPlannerRepository _repository;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public TourPlannerService(IMapper mapper, ITourPlannerRepository repository, IDomainEventDispatcher eventDispatcher)
        {
            _mapper = mapper;
            _repository = repository;
            _eventDispatcher = eventDispatcher;
        }

        public TourPlannerDto Create(long userId, TourPlannerCreateDto dto)
        {
            HandleTourPlannerAchievements(userId);

            EnsureNoOverlappingPlan(userId, dto.TourId, dto.StartDate, dto.EndDate, null);

            var planner = new TourPlanner(
                userId,
                dto.TourId,
                dto.StartDate,
                dto.EndDate
            );

            var created = _repository.Create(planner);
            return _mapper.Map<TourPlannerDto>(created);
        }


        public TourPlannerDto Update(long id, long userId, TourPlannerUpdateDto dto)
        {
            var planner = _repository.GetById(id);
            if (planner.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can not update someone else's planner item.");
            }

            EnsureNoOverlappingPlan(userId, planner.TourId, dto.StartDate, dto.EndDate, planner.Id);
            planner.Update(dto.StartDate, dto.EndDate);
            _repository.Update(planner);
            return _mapper.Map<TourPlannerDto>(planner);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        public TourPlannerDto GetById(long id)
        {
            var planner = _repository.GetById(id);
            return _mapper.Map<TourPlannerDto>(planner);
        }

        public List<TourPlannerDto> GetAllByUserId(long userId)
        {
            var planners = _repository.GetAllByUserId(userId);
            return planners.Select(_mapper.Map<TourPlannerDto>).ToList();
        }

        public PagedResult<TourPlannerDto> GetByUserId(long userId, int page, int size)
        {
            var result = _repository.GetByUserId(userId, page, size);
            var items = result.Results.Select(_mapper.Map<TourPlannerDto>).ToList();
            return new PagedResult<TourPlannerDto>(items, result.TotalCount);
        }

        private void EnsureNoOverlappingPlan(long userId, long tourId, DateTime startDate, DateTime endDate, long? excludeId)
        {
            if (_repository.HasOverlappingPlan(userId, tourId, startDate, endDate, excludeId))
            {
                throw new EntityValidationException("You can not schedule the same tour in an overlapping period.");
            }
        }

        private void HandleTourPlannerAchievements(long userId)
        {
            var count = GetAllByUserId(userId).Count;

            if (count == 0)
            {
                _eventDispatcher
                    .DispatchAsync(new AchievementUnlockedEvent(userId, 4))
                    .GetAwaiter().GetResult();
            }
            else if (count == 9)
            {
                _eventDispatcher
                    .DispatchAsync(new AchievementUnlockedEvent(userId, 5))
                    .GetAwaiter().GetResult();
            }
        }

    }
}
