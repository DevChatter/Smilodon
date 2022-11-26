namespace Smilodon.Domain.Models
{
    public class PreviewCardProvider
    {
        public long Id { get; set; }
        public string Domain { get; set; } = null!;
        public string? IconFileName { get; set; }
        public string? IconContentType { get; set; }
        public long? IconFileSize { get; set; }
        public DateTime? IconUpdatedAt { get; set; }
        public bool? Trendable { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? RequestedReviewAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
