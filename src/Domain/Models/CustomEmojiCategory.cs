namespace Smilodon.Domain.Models
{
    public class CustomEmojiCategory
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
