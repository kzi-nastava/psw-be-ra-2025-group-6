using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Public;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.UseCases
{
    public class JournalService:IJournalService
    {
        private readonly IJournalRepository _journalRepository;
        private readonly IMapper _mapper;

       
        public JournalService(IJournalRepository repository, IMapper mapper)
        {
            _journalRepository = repository;
            _mapper = mapper;
        }
        public async Task<JournalDto> Create(JournalDto journalDto)
        {
            var journal = _mapper.Map<Journal>(journalDto);
            var result = await _journalRepository.Save(journal);
            return _mapper.Map<JournalDto>(result);
        }

        public async Task<JournalDto> Update(JournalDto journalDto)
        {
           
            var existingJournal = await _journalRepository.GetById(journalDto.Id);

            if (existingJournal == null)
                throw new NotFoundException($"Journal not found: {journalDto.Id}");

            
            var type = existingJournal.GetType();

            type.GetProperty("Name")?.SetValue(existingJournal, journalDto.Name);
            type.GetProperty("Location")?.SetValue(existingJournal, journalDto.Location);
            type.GetProperty("TravelDate")?.SetValue(existingJournal, journalDto.TravelDate);

            if (Enum.TryParse<JournalStatus>(journalDto.Status, out var status))
                type.GetProperty("Status")?.SetValue(existingJournal, status);

            type.GetProperty("DateModified")?.SetValue(existingJournal, DateTime.UtcNow);

            
            var result = await _journalRepository.Save(existingJournal);

            
            return new JournalDto
            {
                Id = result.Id,
                TouristId = result.TouristId,
                Name = result.Name,
                Location = result.Location,
                TravelDate = result.TravelDate,
                Status = result.Status.ToString()
            };
        }


        public async Task<List<JournalDto>> GetAllByTouristId(long touristId)
        {
            var journals = await _journalRepository.GetAllByTouristId(touristId);
            return journals.Select(_mapper.Map<JournalDto>).ToList();
        }

        public async Task<JournalDto?> GetById(long journalId)
        {
            var journal = await _journalRepository.GetById(journalId);
            if (journal == null)
                return null;

            return _mapper.Map<JournalDto>(journal);
        }


        public async Task Delete(long journalId)
        {
            await _journalRepository.Delete(journalId);
        }
    }
}
