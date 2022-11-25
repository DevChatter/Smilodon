namespace Smilodon.Domain.Models
{
    public class Webhook
    {
        public long Id { get; set; }
        public string Url { get; set; } = null!;
        public string[] Events { get; set; } = null!;
        public string Secret { get; set; } = null!;
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
