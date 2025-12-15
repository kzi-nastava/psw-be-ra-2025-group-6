using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public
{
    public interface IJournalService
    {
        Task<JournalDto> Create(JournalDto journalDto);
        Task<JournalDto> Update(JournalDto journalDto);
        Task<List<JournalDto>> GetAllByTouristId(long touristId);
        Task<JournalDto> GetById(long journalId);
        Task Delete(long journalId);
    }
}
