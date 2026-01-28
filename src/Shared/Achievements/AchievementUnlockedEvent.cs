using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Achievements
{
    public record AchievementUnlockedEvent(
    long UserId,
    long AchievementId
) : IDomainEvent;

}
