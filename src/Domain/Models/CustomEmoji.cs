namespace Smilodon.Domain.Models
{
    public class CustomEmoji
    {
        public long Id { get; set; }
        public string Shortcode { get; set; } = null!;
        public string? Domain { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageContentType { get; set; }
        public int? ImageFileSize { get; set; }
        public DateTime? ImageUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Disabled { get; set; }
        public string? Uri { get; set; }
        public string? ImageRemoteUrl { get; set; }
        public bool VisibleInPicker { get; set; }
        public long? CategoryId { get; set; }
        public int? ImageStorageSchemaVersion { get; set; }

        public virtual ICollection<AnnouncementReaction> AnnouncementReactions { get; set; } = new HashSet<AnnouncementReaction>();
    }
}
