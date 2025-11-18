using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.ReviewAppDtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ReviewAppService : IReviewAppService
    {
        private readonly IReviewAppRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewAppService(IReviewAppRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public ReviewAppDto Create(CreateReviewAppDto dto)
        {
            var review = new ReviewApp(dto.UserId, dto.Rating, dto.Comment);

            _reviewRepository.Add(review);

            return _mapper.Map<ReviewAppDto>(review);
        }
        public PagedResult<ReviewAppDto> GetPaged(int page, int pageSize)
        {
            var pagedResult = _reviewRepository.GetPaged(page, pageSize);

            return _mapper.Map<PagedResult<ReviewAppDto>>(pagedResult);
        }
        public ReviewAppDto Update(long id, UpdateReviewAppDto dto)
        {
            var review = _reviewRepository.Get(id);
            if (review == null)
                throw new KeyNotFoundException($"Review with id {id} not found.");

            review.Update(dto.Rating, dto.Comment);

            _reviewRepository.Update(review);

            return _mapper.Map<ReviewAppDto>(review);
        }

        public List<ReviewAppDto> GetAll()
        {
            var reviews = _reviewRepository.GetAll();
            return _mapper.Map<List<ReviewAppDto>>(reviews);
        }

        public List<ReviewAppDto> GetByUser(long userId)
        {
            var reviews = _reviewRepository.GetByUser(userId);
            return _mapper.Map<List<ReviewAppDto>>(reviews);
        }
    }
}
