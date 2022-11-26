namespace Smilodon.Domain.Models
{
    public class Conversation
    {
        public long Id { get; set; }
        public string? Uri { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<AccountConversation> AccountConversations { get; set; } = new HashSet<AccountConversation>();
        public virtual ICollection<ConversationMute> ConversationMutes { get; set; } = new HashSet<ConversationMute>();
    }
}
