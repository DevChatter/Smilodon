namespace Smilodon.Domain.Models
{
    public class AccountStatusesCleanupPolicy
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public bool Enabled { get; set; }
        public int MinStatusAge { get; set; }
        public bool KeepDirect { get; set; }
        public bool KeepPinned { get; set; }
        public bool KeepPolls { get; set; }
        public bool KeepMedia { get; set; }
        public bool KeepSelfFav { get; set; }
        public bool KeepSelfBookmark { get; set; }
        public int? MinFavs { get; set; }
        public int? MinReblogs { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
