namespace Smilodon.Domain.Models
{
    public class Invite
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Code { get; set; } = null!;
        public DateTime? ExpiresAt { get; set; }
        public int? MaxUses { get; set; }
        public int Uses { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Autofollow { get; set; }
        public string? Comment { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
