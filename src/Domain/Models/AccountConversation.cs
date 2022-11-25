namespace Smilodon.Domain.Models
{
    public class AccountConversation
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public long? ConversationId { get; set; }
        public long[] ParticipantAccountIds { get; set; } = null!;
        public long[] StatusIds { get; set; } = null!;
        public long? LastStatusId { get; set; }
        public int LockVersion { get; set; }
        public bool Unread { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Conversation? Conversation { get; set; }
    }
}
