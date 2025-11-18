using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases; // For PagedResult
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases;

public class MeetupService : IMeetupService
{
    private readonly IMeetupRepository _meetupRepository;
    private readonly IMapper _mapper;

    public MeetupService(IMeetupRepository meetupRepository, IMapper mapper)
    {
        _meetupRepository = meetupRepository;
        _mapper = mapper;
    }

    public PagedResult<MeetupDto> GetPaged(int page, int pageSize)
    {
        var pagedResult = _meetupRepository.GetPaged(page, pageSize);
        var meetupDtos = _mapper.Map<List<MeetupDto>>(pagedResult.Results);
        return new PagedResult<MeetupDto>(meetupDtos, pagedResult.TotalCount);
    }

    public MeetupDto Get(long id)
    {
        var meetup = _meetupRepository.Get(id);
        return _mapper.Map<MeetupDto>(meetup);
    }

    public MeetupDto Create(MeetupDto meetupDto)
    {
        var meetup = _mapper.Map<Meetup>(meetupDto);
        var createdMeetup = _meetupRepository.Create(meetup);
        return _mapper.Map<MeetupDto>(createdMeetup);
    }

    public MeetupDto Update(MeetupDto meetupDto)
    {
        var meetup = _mapper.Map<Meetup>(meetupDto);
        var updatedMeetup = _meetupRepository.Update(meetup);
        return _mapper.Map<MeetupDto>(updatedMeetup);
    }

    public void Delete(long id)
    {
        _meetupRepository.Delete(id);
    }
}
