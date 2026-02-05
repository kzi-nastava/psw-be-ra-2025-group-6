using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IClubMemberRepository _clubMemberRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public ClubService(
            IClubRepository clubRepository,
            IClubMemberRepository clubMemberRepository,
            IPersonRepository personRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IMapper mapper)
        {
            _clubRepository = clubRepository;
            _clubMemberRepository = clubMemberRepository;
            _personRepository = personRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public ClubDto Create(ClubDto clubDto)
        {
            var clubDomain = _mapper.Map<Club>(clubDto);
            var result = _clubRepository.Create(clubDomain);
            return _mapper.Map<ClubDto>(result);
        }

        public ClubDto Update(ClubDto clubDto)
        {
            var clubDomain = _mapper.Map<Club>(clubDto);
            var result = _clubRepository.Update(clubDomain);
            return _mapper.Map<ClubDto>(result);
        }

        public void Delete(long id)
        {
            var club = _clubRepository.Get(id);
            if (club.Status == ClubStatus.Active)
            {
                throw new InvalidOperationException("Club must be closed before deletion.");
            }
            _clubRepository.Delete(id);
        }

        public ClubDto Get(long id)
        {
            var result = _clubRepository.Get(id);
            return _mapper.Map<ClubDto>(result);
        }

        public List<ClubDto> GetAll()
        {
            var result = _clubRepository.GetAll();
            return _mapper.Map<List<ClubDto>>(result);
        }

        // Owner controls
        public ClubDto ChangeStatus(long clubId, string status, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            
            if (!Enum.TryParse<ClubStatus>(status, true, out var newStatus))
                throw new ArgumentException($"Invalid status: {status}");

            club.ChangeStatus(newStatus, ownerId);
            
            var updated = _clubRepository.Update(club);
            return _mapper.Map<ClubDto>(updated);
        }

        public List<ClubMemberDto> GetMembers(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the owner can view members");

            var members = _clubMemberRepository.GetByClubId(clubId);
            var memberDtos = new List<ClubMemberDto>();

            foreach (var member in members)
            {
                var person = _personRepository.GetById(member.UserId);
                memberDtos.Add(new ClubMemberDto
                {
                    Id = member.Id,
                    ClubId = member.ClubId,
                    UserId = member.UserId,
                    UserName = person.Name + " " + person.Surname,
                    UserEmail = person.Email,
                    JoinedAt = member.JoinedAt,
                    Status = member.Status.ToString()
                });
            }

            return memberDtos;
        }

        public ClubMemberDto InviteMember(long clubId, string username, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the owner can invite members");

            if (!club.CanAcceptMembers())
                throw new InvalidOperationException("Club is closed and cannot accept new members");

            // Find user by username
            var user = _userRepository.GetActiveByName(username);
            if (user == null)
                throw new KeyNotFoundException($"User not found with username: {username}");

            // Get person ID for the user
            var userId = _userRepository.GetPersonId(user.Id);

            if (_clubMemberRepository.IsMember(clubId, userId))
                throw new InvalidOperationException("User is already a member");

            var member = new ClubMember(clubId, userId);
            var created = _clubMemberRepository.Create(member);

            var person = _personRepository.GetById(userId);
            
            // Send notification to the invited member
            var notificationContent = $"You have been invited to join the club '{club.Name}'.";
            _notificationService.Create(new NotificationDto
            {
                RecipientId = userId,
                SenderId = ownerId,
                Content = notificationContent,
                Status = "Unread",
                Timestamp = DateTime.UtcNow,
                ReferenceId = clubId
            });

            return new ClubMemberDto
            {
                Id = created.Id,
                ClubId = created.ClubId,
                UserId = created.UserId,
                UserName = person.Name + " " + person.Surname,
                UserEmail = person.Email,
                JoinedAt = created.JoinedAt,
                Status = created.Status.ToString()
            };
        }

        public void RemoveMember(long clubId, long memberId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the owner can remove members");

            if (!club.CanAcceptMembers())
                throw new InvalidOperationException("Club is closed");

            // Get member details before deletion for notification
            var member = _clubMemberRepository.Get(memberId);
            if (member == null)
                throw new KeyNotFoundException("Member not found");

            if (member.ClubId != clubId)
                throw new InvalidOperationException("Member does not belong to this club");

            // Delete the member
            _clubMemberRepository.Delete(memberId);

            // Send notification to the removed member
            var notificationContent = $"You have been removed from the club '{club.Name}'.";
            _notificationService.Create(new NotificationDto
            {
                RecipientId = member.UserId,
                SenderId = ownerId,
                Content = notificationContent,
                Status = "Unread",
                Timestamp = DateTime.UtcNow,
                ReferenceId = clubId
            });
        }
    }
}