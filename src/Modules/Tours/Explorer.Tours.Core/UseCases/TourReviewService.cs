using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases
{
    public class TourReviewService : ITourReviewService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public TourReviewService(ITourRepository tourRepository, IMapper mapper)
        {
             _tourRepository = tourRepository;
             _mapper = mapper;
        }

        public TourReviewDto Create(TourReviewDto tourRev)
        {
            var review = new TourReview(
               tourRev.UserId,
               tourRev.TourId,
               tourRev.Rating,
               tourRev.Comment,
               tourRev.CompletedPercent
           );
            var tour = _tourRepository.Get(tourRev.TourId);
            tour.AddTourReview(review);
            _tourRepository.Update(tour);
            return _mapper.Map<TourReviewDto>(review);
        }

        public TourReviewDto Update(TourReviewDto tourRev)
        {
            var tour = _tourRepository.GetByReviewId(tourRev.Id);
            if (tour == null) throw new NotFoundException("Tour review not found: " + tourRev.Id);

            var rev = tour.TourReviews.FirstOrDefault(r => r.Id == tourRev.Id);
            rev.Update(tourRev.Rating, tourRev.Comment, tourRev.CompletedPercent);

            _tourRepository.Update(tour);
            return _mapper.Map<TourReviewDto>(rev);
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
             return _mapper.Map<TourReviewDto>(rev);
        }

        public List<TourReviewDto> GetByUser(long userId)
        {
            var tours = _tourRepository.GetByReviewUserId(userId);
            var reviews = tours.SelectMany(t => t.TourReviews).Where(r => r.UserId == userId).ToList();

            var reviewDtos = new List<TourReviewDto>();
            foreach (var rev in reviews)
            {
                reviewDtos.Add(_mapper.Map<TourReviewDto>(rev));
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
                 reviewDtos.Add(_mapper.Map<TourReviewDto>(rev));
             }
             return reviewDtos;
        }
    }
}
