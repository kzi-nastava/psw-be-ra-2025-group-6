using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class MembershipRequestService : IMembershipRequestService
    {
        private readonly IClubMembershipRequestRepository _requestRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IClubMemberRepository _memberRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;

        public MembershipRequestService(
            IClubMembershipRequestRepository requestRepository,
            IClubRepository clubRepository,
            IClubMemberRepository memberRepository,
            INotificationService notificationService,
            IUserProfileRepository userProfileRepository,
            IMapper mapper)
        {
            _requestRepository = requestRepository;
            _clubRepository = clubRepository;
            _memberRepository = memberRepository;
            _notificationService = notificationService;
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }

        public ClubMembershipRequestDto SendRequest(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);

            if (!club.CanAcceptMembers())
                throw new InvalidOperationException("Club is not active and cannot accept new members.");

            var existing = _requestRepository.GetActive(clubId, touristId);
            if (existing != null)
                throw new InvalidOperationException("You already have a pending request for this club.");

            var request = new ClubMembershipRequest(clubId, touristId);
            var result = _requestRepository.Create(request);

            return _mapper.Map<ClubMembershipRequestDto>(result);
        }

        public void WithdrawRequest(long requestId, long userId)
        {
            var request = _requestRepository.Get(requestId);
            if (request.TouristId != userId)
                throw new UnauthorizedAccessException("You can only withdraw your own requests.");

            _requestRepository.Delete(requestId);
        }

        public List<ClubMembershipRequestDto> GetPendingRequestsByClub(long clubId, long userId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null) throw new KeyNotFoundException("Club not found");

            var allRequests = _requestRepository.GetByClub(clubId);
            List<ClubMembershipRequest> filteredRequests;

            if (club.OwnerId == userId)
            {
                filteredRequests = allRequests;
            }
            else
            {
                filteredRequests = allRequests.Where(r => r.TouristId == userId).ToList();
            }

            var dtos = _mapper.Map<List<ClubMembershipRequestDto>>(filteredRequests);

            foreach (var dto in dtos)
            {
                var profile = _userProfileRepository.GetAll().FirstOrDefault(p => p.UserId == dto.TouristId);
                dto.TouristUsername = profile != null ? $"{profile.Name} {profile.Surname}" : $"User #{dto.TouristId}";
            }

            return dtos;
        }

        public void AcceptRequest(long requestId, long ownerId)
        {
            var request = _requestRepository.Get(requestId);
            var club = _clubRepository.Get(request.ClubId);

            if (club.OwnerId != ownerId) throw new UnauthorizedAccessException();

            var newMember = new ClubMember(request.ClubId, request.TouristId);
            _memberRepository.Create(newMember);

            _notificationService.Create(new NotificationDto
            {
                RecipientId = request.TouristId,
                SenderId = ownerId,
                Content = $"Your request for club '{club.Name}' has been accepted!",
                ReferenceId = club.Id
            });

            _requestRepository.Delete(requestId);
        }

        public void RejectRequest(long requestId, long ownerId)
        {
            var request = _requestRepository.Get(requestId);
            var club = _clubRepository.Get(request.ClubId);

            if (club.OwnerId != ownerId) throw new UnauthorizedAccessException();

            _notificationService.Create(new NotificationDto
            {
                RecipientId = request.TouristId,
                SenderId = ownerId,
                Content = $"Your request for club '{club.Name}' was rejected.",
                ReferenceId = club.Id
            });

            _requestRepository.Delete(requestId);
        }
    }
}