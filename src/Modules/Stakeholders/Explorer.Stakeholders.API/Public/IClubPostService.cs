using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubPostService
    {
        ClubPostDto Create(ClubPostDto post, long userId);
        ClubPostDto Update(ClubPostDto post, long userId);
        void Delete(long id, long userId);
        ClubPostDto Get(long id);
        List<ClubPostDto> GetForClub(long clubId);
    }
}
