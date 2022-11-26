namespace Smilodon.Domain.Models
{
    public class AccountWarningPreset
    {
        public long Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Title { get; set; } = null!;
    }
}
