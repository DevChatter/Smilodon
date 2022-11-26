namespace Smilodon.Domain.Models
{
    public class Identity
    {
        public long Id { get; set; }
        public string Provider { get; set; } = null!;
        public string Uid { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
