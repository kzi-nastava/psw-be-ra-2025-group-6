using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TourProblemService : ITourProblemService
    {
        private readonly ITourProblemRepository _repository;
        private readonly ITourInfoGateway _tourInfoGateway;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public TourProblemService(ITourProblemRepository repository, ITourInfoGateway tourInfoGateway, INotificationService notificationService, IMapper mapper)
        {
            _repository = repository;
            _tourInfoGateway = tourInfoGateway;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<List<TourProblemDto>> GetAll()
        {
            var problems = await _repository.GetAll();
            return problems.Select(_mapper.Map<TourProblemDto>).ToList();
        }

        public async Task<TourProblemDto> Create(TourProblemDto problemDto)
        {
            var problem = new TourProblem(
                problemDto.TourId,
                problemDto.TouristId,
                (ProblemCategory)problemDto.Category,
                (ProblemPriority)problemDto.Priority,
                problemDto.Description
            );

            var result = await _repository.Create(problem);
            return _mapper.Map<TourProblemDto>(result);
        }

        public async Task<List<TourProblemDto>> GetByTourist(long touristId)
        {
            var problems = await _repository.GetByTourist(touristId);
            return problems.Select(_mapper.Map<TourProblemDto>).ToList();
        }

        public async Task<List<TourProblemDto>> GetByAuthor(long authorId)
        {
            var authorTours = await _tourInfoGateway.GetByAuthor(authorId);
            if (!authorTours.Any()) return new List<TourProblemDto>();
            var tourIds = authorTours.Select(t => t.Id).ToList();
            var problems = await _repository.GetByTourIds(tourIds);
            return problems.Select(_mapper.Map<TourProblemDto>).ToList();
        }

        public async Task<TourProblemDto> Update(TourProblemDto problemDto)
        {
            var existingProblem = await _repository.GetById(problemDto.Id);

            if (existingProblem == null)
                throw new KeyNotFoundException($"Problem with ID {problemDto.Id} not found.");

            existingProblem.Update(
                (ProblemCategory)problemDto.Category,
                (ProblemPriority)problemDto.Priority,
                problemDto.Description
            );

            var result = await _repository.Update(existingProblem);
            return _mapper.Map<TourProblemDto>(result);
        }

        public async Task<TourProblemDto> SetDeadline(long id, DateTime deadlineUtc, long adminPersonId)
        {
            var problem = await _repository.GetById(id);

            if (problem == null)
                throw new KeyNotFoundException($"Problem with ID {id} not found.");

            problem.SetDeadline(deadlineUtc);

            var result = await _repository.Update(problem);

            var tour = await _tourInfoGateway.GetById(problem.TourId);
            if (tour == null)
                throw new KeyNotFoundException($"Tour with ID {problem.TourId} not found.");

            var content = $"The administrator has set a deadline of {deadlineUtc:yyyy-MM-dd} for resolving problem #{problem.Id} on the tour '{tour.Name}'. If you do not respond or take the required action by the specified deadline, appropriate penalties may be applied.";

            _notificationService.Create(new NotificationDto
            {
                RecipientId = tour.AuthorId,
                SenderId = adminPersonId,
                Content = content,
                ReferenceId = problem.Id
            });

            return _mapper.Map<TourProblemDto>(result);
        }

        public async Task<TourProblemDto> SetResolutionFeedback(long id, TourProblemResolutionDto resolutionDto, long touristPersonId)
        {
            var problem = await _repository.GetById(id);

            if (problem == null)
                throw new NotFoundException($"Problem with ID {id} not found.");

            if (problem.TouristId != touristPersonId)
                throw new ForbiddenException("You can only update resolution feedback for your own problems.");

            var feedback = (ProblemResolutionFeedback)resolutionDto.Feedback;
            problem.SetResolutionFeedback(feedback, resolutionDto.Comment);

            var result = await _repository.Update(problem);

            var tour = await _tourInfoGateway.GetById(problem.TourId);
            if (tour != null)
            {
                var content = feedback == ProblemResolutionFeedback.ResolvedByTourist
                    ? $"Tourist marked problem #{problem.Id} on '{tour.Name}' as resolved."
                    : $"Tourist marked problem #{problem.Id} on '{tour.Name}' as not resolved: {resolutionDto.Comment}";

                _notificationService.Create(new NotificationDto
                {
                    RecipientId = tour.AuthorId,
                    SenderId = touristPersonId,
                    Content = content,
                    ReferenceId = problem.Id
                });
            }

            return _mapper.Map<TourProblemDto>(result);
        }

        public async Task<TourProblemDto> FinalizeStatus(long id, int status, long adminPersonId)
        {
            var problem = await _repository.GetById(id);

            if (problem == null)
                throw new NotFoundException($"Problem with ID {id} not found.");

            if (!Enum.IsDefined(typeof(ProblemStatus), status))
                throw new ArgumentException("Unknown status.");

            var statusEnum = (ProblemStatus)status;

            problem.FinalizeStatus(statusEnum, DateTime.UtcNow);

            var result = await _repository.Update(problem);

            var tour = await _tourInfoGateway.GetById(problem.TourId);
            if (tour == null)
                throw new NotFoundException($"Tour with ID {problem.TourId} not found.");

            if (statusEnum == ProblemStatus.Closed)
            {
                _notificationService.Create(new NotificationDto
                {
                    RecipientId = tour.AuthorId,
                    SenderId = adminPersonId,
                    Content = $"Thank you for resolving the issue reported by the tourist (ID: {problem.TouristId}) on tour (ID: {problem.TourId}) in a timely manner.",
                    ReferenceId = problem.Id
                });

                _notificationService.Create(new NotificationDto
                {
                    RecipientId = problem.TouristId,
                    SenderId = adminPersonId,
                    Content = "Your reported issue has been successfully resolved. Thank you for your cooperation.",
                    ReferenceId = problem.Id
                });
            }
            else if (statusEnum == ProblemStatus.Penalized)
            {
                _notificationService.Create(new NotificationDto
                {
                    RecipientId = tour.AuthorId,
                    SenderId = adminPersonId,
                    Content = $"The tourist (ID: {problem.TouristId}) marked that the issue on tour (ID: {problem.TourId}) was not adequately resolved. Penalties have been applied to this tour based on the report. Repeated reports may lead to stricter administrative measures.",
                    ReferenceId = problem.Id
                });

                _notificationService.Create(new NotificationDto
                {
                    RecipientId = problem.TouristId,
                    SenderId = adminPersonId,
                    Content = "Your feedback has been recorded. Since you marked the issue as not resolved, the tour and its author have been sanctioned according to the platform rules. Thank you for helping maintain tour quality.",
                    ReferenceId = problem.Id
                });

                var penalizedCount = await _repository.CountByTourAndStatus(problem.TourId, ProblemStatus.Penalized);
                if (penalizedCount >= 3)
                {
                    await _tourInfoGateway.SuspendTour(problem.TourId);
                    _notificationService.Create(new NotificationDto
                    {
                        RecipientId = tour.AuthorId,
                        SenderId = adminPersonId,
                        Content = $"Tour (ID: {problem.TourId}) has been suspended due to repeated unresolved issues.",
                        ReferenceId = problem.Id
                    });
                }
            }

            return _mapper.Map<TourProblemDto>(result);
        }

        public async Task Delete(long id, long touristId)
        {
            var problem = await _repository.GetById(id);

            if (problem == null)
                throw new KeyNotFoundException($"Problem with ID {id} not found.");

            if (problem.TouristId != touristId)
                throw new UnauthorizedAccessException("You can only delete your own problems.");

            await _repository.Delete(id);
        }
    }
}
