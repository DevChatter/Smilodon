namespace Smilodon.Domain.Models
{
    public class WebSetting
    {
        public long Id { get; set; }
        public string? Data { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long UserId { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
