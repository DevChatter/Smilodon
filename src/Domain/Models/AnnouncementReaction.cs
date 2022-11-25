namespace Smilodon.Domain.Models
{
    public class AnnouncementReaction
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public long? AnnouncementId { get; set; }
        public string Name { get; set; } = null!;
        public long? CustomEmojiId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Announcement? Announcement { get; set; }
        public virtual CustomEmoji? CustomEmoji { get; set; }
    }
}
