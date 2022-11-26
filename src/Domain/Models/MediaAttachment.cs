namespace Smilodon.Domain.Models
{
    public class MediaAttachment
    {
        public long Id { get; set; }
        public long? StatusId { get; set; }
        public string? FileFileName { get; set; }
        public string? FileContentType { get; set; }
        public int? FileFileSize { get; set; }
        public DateTime? FileUpdatedAt { get; set; }
        public string RemoteUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Shortcode { get; set; }
        public int Type { get; set; }
        public string? FileMeta { get; set; }
        public long? AccountId { get; set; }
        public string? Description { get; set; }
        public long? ScheduledStatusId { get; set; }
        public string? Blurhash { get; set; }
        public int? Processing { get; set; }
        public int? FileStorageSchemaVersion { get; set; }
        public string? ThumbnailFileName { get; set; }
        public string? ThumbnailContentType { get; set; }
        public int? ThumbnailFileSize { get; set; }
        public DateTime? ThumbnailUpdatedAt { get; set; }
        public string? ThumbnailRemoteUrl { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ScheduledStatus? ScheduledStatus { get; set; }
        public virtual Status? Status { get; set; }
    }
}
