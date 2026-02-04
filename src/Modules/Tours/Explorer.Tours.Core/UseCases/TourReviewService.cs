using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Tours.Core.UseCases
{
    public class TourReviewService : ITourReviewService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;
        private readonly IInternalStakeholderService _stakeholderService;
        private readonly ITourReviewHelpfulRepository _helpfulRepository;

        public TourReviewService(
            ITourRepository tourRepository,
            IMapper mapper,
            IInternalStakeholderService stakeholderService,
            ITourReviewHelpfulRepository helpfulRepository)
        {
            _tourRepository = tourRepository;
            _mapper = mapper;
            _stakeholderService = stakeholderService;
            _helpfulRepository = helpfulRepository;
        }

        public TourReviewDto Create(TourReviewDto tourRev)
        {
            var review = new TourReview(
               tourRev.UserId,
               tourRev.TourId,
               tourRev.Rating,
               tourRev.Comment,
               tourRev.CompletedPercent,
               tourRev.PictureUrl
           );

            var tour = _tourRepository.Get(tourRev.TourId);
            tour.AddTourReview(review);
            _tourRepository.Update(tour);

            return MapWithUserName(review);
        }

        public TourReviewDto Update(TourReviewDto tourRev)
        {
            var tour = _tourRepository.GetByReviewId(tourRev.Id);
            if (tour == null) throw new NotFoundException("Tour review not found: " + tourRev.Id);

            var rev = tour.TourReviews.FirstOrDefault(r => r.Id == tourRev.Id);
            rev.Update(tourRev.Rating, tourRev.Comment, tourRev.CompletedPercent, tourRev.PictureUrl);

            _tourRepository.Update(tour);
            return MapWithUserName(rev);
        }

        public void Delete(long id)
        {
            var tour = _tourRepository.GetByReviewId(id);
            if (tour == null) throw new NotFoundException("Tour review not found: " + id);

            var rev = tour.TourReviews.FirstOrDefault(r => r.Id == id);
            tour.TourReviews.Remove(rev);
            _tourRepository.Update(tour);
        }

        public TourReviewDto Get(long id)
        {
            var tour = _tourRepository.GetByReviewId(id);
            if (tour == null) throw new NotFoundException("Tour review not found: " + id);
            var rev = tour.TourReviews.FirstOrDefault(r => r.Id == id);
            return MapWithUserName(rev);
        }

        public List<TourReviewDto> GetByUser(long userId)
        {
            var tours = _tourRepository.GetByReviewUserId(userId);
            var reviews = tours.SelectMany(t => t.TourReviews).Where(r => r.UserId == userId).ToList();

            var reviewDtos = new List<TourReviewDto>();
            foreach (var rev in reviews)
            {
                reviewDtos.Add(MapWithUserName(rev));
            }
            return reviewDtos;
        }

        public List<TourReviewDto> GetByTour(long tourId)
        {
            var tour = _tourRepository.Get(tourId);
            var reviews = tour.TourReviews;

            var reviewDtos = new List<TourReviewDto>();
            foreach (var rev in reviews)
            {
                reviewDtos.Add(MapWithUserName(rev));
            }
            return reviewDtos;
        }

        public (int helpfulCount, bool isHelpful) ToggleHelpful(long reviewId, long userId)
        {
            // Validate review exists
            var tour = _tourRepository.GetByReviewId(reviewId);
            if (tour == null) throw new NotFoundException("Tour review not found: " + reviewId);

            // If already exists remove (toggle off), otherwise add
            var already = _helpfulRepository.Exists(reviewId, userId);
            if (already)
            {
                _helpfulRepository.RemoveByReviewAndUser(reviewId, userId);
                var countAfter = _helpfulRepository.CountByReview(reviewId);
                return (countAfter, false);
            }
            else
            {
                var vote = new TourReviewHelpfulVote(reviewId, userId);
                _helpfulRepository.Add(vote);
                var countAfter = _helpfulRepository.CountByReview(reviewId);
                return (countAfter, true);
            }
        }

        // NEW: explicit remove helpful (used by DELETE endpoint)
        public (int helpfulCount, bool isHelpful) RemoveHelpful(long reviewId, long userId)
        {
            var tour = _tourRepository.GetByReviewId(reviewId);
            if (tour == null) throw new NotFoundException("Tour review not found: " + reviewId);

            var exists = _helpfulRepository.Exists(reviewId, userId);
            if (!exists)
            {
                // nothing to remove; return current count and isHelpful=false
                return (_helpfulRepository.CountByReview(reviewId), false);
            }

            _helpfulRepository.RemoveByReviewAndUser(reviewId, userId);
            var countAfter = _helpfulRepository.CountByReview(reviewId);
            return (countAfter, false);
        }

        private TourReviewDto MapWithUserName(TourReview rev)
        {
            var dto = _mapper.Map<TourReviewDto>(rev);
            try
            {
                // IInternalStakeholderService returns a username string; use it instead of returning full user
                dto.UserName = _stakeholderService.GetUsername(rev.UserId) ?? "Anonymous";
            }
            catch
            {
                dto.UserName = "Anonymous";
            }

            // Populate helpful count
            try
            {
                dto.HelpfulCount = _helpfulRepository.CountByReview(rev.Id);
            }
            catch
            {
                dto.HelpfulCount = 0;
            }

            return dto;
        }
    }
}
