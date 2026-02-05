using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Shared.Achievements;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class AchievementUnlockedHandler
    : IDomainEventHandler<AchievementUnlockedEvent>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAchievementRepository _achievementRepo;

        public AchievementUnlockedHandler(
            IUserProfileService userProfileService,
            IAchievementRepository achievementRepo)
        {
            _userProfileService = userProfileService;
            _achievementRepo = achievementRepo;
        }

        public async Task Handle(AchievementUnlockedEvent e)
        {
            var achievement = _achievementRepo.GetById(e.AchievementId);
            if (achievement == null) return;

            _userProfileService.AddAchievement(e.UserId, achievement.Id);
        }
    }

}
