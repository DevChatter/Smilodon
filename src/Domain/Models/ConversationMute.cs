namespace Smilodon.Domain.Models
{
    public class ConversationMute
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long AccountId { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Conversation Conversation { get; set; } = null!;
    }
}
