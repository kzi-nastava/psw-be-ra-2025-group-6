using AutoMapper;
using System;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.Core.Mappers
{
    public class EncountersProfile : Profile 
    {
        public EncountersProfile()
        {
            CreateMap<Challenge, ChallengeDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
                .ForMember(d => d.ImagePath, opt => opt.MapFrom(s => s.ImagePath));

            CreateMap<ChallengeDto, Challenge>().ConvertUsing(new ChallengeDtoToChallengeConverter());

            CreateMap<EncounterCompletion, EncounterCompletionDto>().ReverseMap();

            CreateMap<SocialEncounter, SocialEncounterDto>().ReverseMap();
            CreateMap<ActiveSocialParticipant, ActiveSocialParticipantDto>().ReverseMap();
            
            CreateMap<TouristXpProfile, TouristXpProfileDto>()
                .ForMember(d => d.XpProgressInCurrentLevel, opt => opt.MapFrom(s => s.GetXPProgressInCurrentLevel()))
                .ForMember(d => d.XpNeededForNextLevel, opt => opt.MapFrom(s => s.GetXPNeededForNextLevel()))
                .ForMember(d => d.XpRequiredForNextLevel, opt => opt.MapFrom(s => s.GetXPRequiredForNextLevel()))
                .ForMember(d => d.CanCreateEncounters, opt => opt.MapFrom(s => s.CanCreateEncounters()))
                .ForMember(d => d.LevelUpHistory, opt => opt.MapFrom(s => s.GetLevelUpHistory()));

            // HiddenLocationAttempt mappings
            CreateMap<HiddenLocationAttempt, HiddenLocationAttemptDto>().ReverseMap();
            
            // Leaderboard mappings
            CreateMap<LeaderboardEntry, LeaderboardEntryDto>().ReverseMap();
            CreateMap<ClubLeaderboard, ClubLeaderboardDto>().ReverseMap();
        }

        private class ChallengeDtoToChallengeConverter : ITypeConverter<ChallengeDto, Challenge>
        {
            public Challenge Convert(ChallengeDto source, Challenge destination, ResolutionContext context)
            {
                if (source == null) return null!;

                if (!Enum.TryParse<ChallengeType>(source.Type, true, out var parsedType))
                    throw new ArgumentException("Invalid challenge type.");

                var parsedStatus = ChallengeStatus.Draft;
                if (!string.IsNullOrWhiteSpace(source.Status))
                {
                    if (!Enum.TryParse<ChallengeStatus>(source.Status, true, out parsedStatus))
                        parsedStatus = ChallengeStatus.Draft;
                }

                return new Challenge(
                    source.Title,
                    source.Description,
                    source.Longitude,
                    source.Latitude,
                    source.XP,
                    parsedType,
                    source.ImagePath,
                    source.ActivationRadiusMeters > 0 ? source.ActivationRadiusMeters : 50,
                    parsedStatus
                );
            }
        }
    }
}
