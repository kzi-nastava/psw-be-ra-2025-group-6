using Explorer.Encounters.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Public
{
    public interface ISocialEncounterService
    {
        // Admin methods - za kreiranje i upravljanje Social Encounter-ima
        SocialEncounterDto CreateSocialEncounter(SocialEncounterDto dto);
        SocialEncounterDto UpdateSocialEncounter(long id, SocialEncounterDto dto);
        SocialEncounterDto GetSocialEncounter(long id);
        SocialEncounterDto GetByChallengeId(long challengeId);
        List<SocialEncounterDto> GetAll();
        void DeleteSocialEncounter(long id);

        // Tourist methods - za aktiviranje i praćenje
        ActivateSocialEncounterResponseDto ActivateSocialEncounter(long challengeId, long userId, ActivateSocialEncounterRequestDto request);
        SocialEncounterHeartbeatResponseDto SendHeartbeat(long challengeId, long userId, SocialEncounterHeartbeatRequestDto request);
        DeactivateSocialEncounterResponseDto DeactivateSocialEncounter(long challengeId, long userId);
    }
}
