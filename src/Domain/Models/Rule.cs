namespace Smilodon.Domain.Models
{
    public class Rule
    {
        public long Id { get; set; }
        public int Priority { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
