using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.UseCases;
using System;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubPostService : IClubPostService
    {
        private readonly IClubPostRepository _clubPostRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IClubMembershipService _clubMembershipService;
        private readonly IMapper _mapper;

        public ClubPostService(IClubPostRepository clubPostRepository, IClubRepository clubRepository, IClubMembershipService clubMembershipService, IMapper mapper)
        {
            _clubPostRepository = clubPostRepository;
            _clubRepository = clubRepository;
            _clubMembershipService = clubMembershipService;
            _mapper = mapper;
        }

        public ClubPostDto Create(ClubPostDto postDto, long userId)
        {
            if (!_clubMembershipService.IsMember(userId, postDto.ClubId))
            {
                throw new UnauthorizedAccessException("User is not a member of the club.");
            }
            postDto.AuthorId = userId;
            var post = new ClubPost(postDto.AuthorId, postDto.ClubId, postDto.Text, postDto.ResourceId, MapResourceType(postDto.ResourceType), DateTime.UtcNow, null);
            var result = _clubPostRepository.Create(post);
            return _mapper.Map<ClubPostDto>(result);
        }

        public void Delete(long id, long userId)
        {
            var post = _clubPostRepository.Get(id);
            var club = _clubRepository.Get(post.ClubId);
            if (club.OwnerId != userId)
            {
                throw new UnauthorizedAccessException("User is not the owner of the club.");
            }
            _clubPostRepository.Delete(id);
        }

        public ClubPostDto Get(long id)
        {
            var result = _clubPostRepository.Get(id);
            return _mapper.Map<ClubPostDto>(result);
        }

        public List<ClubPostDto> GetForClub(long clubId)
        {
            var result = _clubPostRepository.GetAllForClub(clubId);
            return _mapper.Map<List<ClubPostDto>>(result);
        }

        public ClubPostDto Update(ClubPostDto postDto, long userId)
        {
            var post = _clubPostRepository.Get(postDto.Id);
            if (post.AuthorId != userId)
            {
                throw new UnauthorizedAccessException("User is not the author of the post.");
            }
            post.Update(postDto.Text, postDto.ResourceId, MapResourceType(postDto.ResourceType), DateTime.UtcNow);
            var result = _clubPostRepository.Update(post);
            return _mapper.Map<ClubPostDto>(result);
        }

        private static ResourceType? MapResourceType(API.Dtos.ResourceTypeDto? dto)
        {
            return dto.HasValue ? (ResourceType?)dto.Value : null;
        }
    }
}
