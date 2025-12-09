using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface ITourProblemService
    {
        Task<List<TourProblemDto>> GetAll();
        Task<TourProblemDto> Create(TourProblemDto problemDto);
        Task<List<TourProblemDto>> GetByTourist(long touristId);
        Task<List<TourProblemDto>> GetByAuthor(long authorId);
        Task<TourProblemDto> Update(TourProblemDto problemDto);
        Task<TourProblemDto> SetDeadline(long id, DateTime deadlineUtc, long adminPersonId);
        Task<TourProblemDto> SetResolutionFeedback(long id, TourProblemResolutionDto resolutionDto, long touristPersonId);
        Task<TourProblemDto> FinalizeStatus(long id, int status, long adminPersonId);
        Task Delete(long id, long touristId);
    }
}
