using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class Follow : Entity
{
    public long FollowerId { get; init; }  
    public long FollowedId { get; init; }  

    public Follow(long followerId, long followedId)
    {
        FollowerId = followerId;
        FollowedId = followedId;
        Validate();
    }

    private void Validate()
    {
        if (FollowerId == 0) throw new ArgumentException("Invalid FollowerId");
        if (FollowedId == 0) throw new ArgumentException("Invalid FollowedId");
        if (FollowerId == FollowedId) throw new ArgumentException("Cannot follow yourself");
    }
}
