using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases
{
    public class TourReviewService : ITourReviewService
    {
        private readonly ITourReviewRepository _tourReviewRepository;
        private readonly IMapper _mapper;

        public TourReviewService(ITourReviewRepository reviewRepository, IMapper mapper)
        {
            _tourReviewRepository = reviewRepository;
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

            _tourReviewRepository.Create(review);
            return _mapper.Map<TourReviewDto>(review);
        }
        public TourReviewDto Update(TourReviewDto tourRev)
        {
            var rev = _tourReviewRepository.Get(tourRev.Id);

            rev.Equals(tourRev);
            _tourReviewRepository.Update(rev);
            return _mapper.Map<TourReviewDto>(rev);

        }
        public void Delete(long id)
        {
            TourReview rev = _tourReviewRepository.Get(id);
            _tourReviewRepository.Delete(rev);
        }
        public TourReviewDto Get(long id)
        {
            var tourReview = _tourReviewRepository.Get(id);
            return _mapper.Map<TourReviewDto>(tourReview);
        }
        public List<TourReviewDto> GetByUser(long userId)
        {
            List<TourReview> reviews = _tourReviewRepository.GetByUser(userId);
            List<TourReviewDto> reviewDtos = new List<TourReviewDto>();
            foreach (TourReview rev in reviews)
            {
                reviewDtos.Add(_mapper.Map<TourReviewDto>(rev));
            }
            return reviewDtos;
        }
        public List<TourReviewDto> GetByTour(long tourId)
        {
            List<TourReview> reviews = _tourReviewRepository.GetByTour(tourId);
            List<TourReviewDto> reviewDtos = new List<TourReviewDto>();
            foreach (TourReview rev in reviews)
            {
                reviewDtos.Add(_mapper.Map<TourReviewDto>(rev));
            }
            return reviewDtos;
        }
    }
}
