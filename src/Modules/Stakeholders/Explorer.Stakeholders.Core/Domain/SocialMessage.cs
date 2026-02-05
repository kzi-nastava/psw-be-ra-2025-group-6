using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class SocialMessage : Entity
{
    public long SenderId { get; private set; }
    public long ReceiverId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Content { get; private set; }

    private SocialMessage() { }

    public SocialMessage(long senderId, long receiverId, string content)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        Timestamp = DateTime.UtcNow;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Content)) throw new ArgumentException("Message empty");
        if (SenderId == ReceiverId) throw new ArgumentException("Cannot message yourself");
    }
}
