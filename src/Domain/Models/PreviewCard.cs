namespace Smilodon.Domain.Models
{
    public class PreviewCard
    {
        public long Id { get; set; }
        public string Url { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageFileName { get; set; }
        public string? ImageContentType { get; set; }
        public int? ImageFileSize { get; set; }
        public DateTime? ImageUpdatedAt { get; set; }
        public int Type { get; set; }
        public string Html { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public string AuthorUrl { get; set; } = null!;
        public string ProviderName { get; set; } = null!;
        public string ProviderUrl { get; set; } = null!;
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string EmbedUrl { get; set; } = null!;
        public int? ImageStorageSchemaVersion { get; set; }
        public string? Blurhash { get; set; }
        public string? Language { get; set; }
        public double? MaxScore { get; set; }
        public DateTime? MaxScoreAt { get; set; }
        public bool? Trendable { get; set; }
        public int? LinkType { get; set; }

        public virtual PreviewCardTrend PreviewCardTrend { get; set; } = null!;
    }
}
