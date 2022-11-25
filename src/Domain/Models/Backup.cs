namespace Smilodon.Domain.Models
{
    public class Backup
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string? DumpFileName { get; set; }
        public string? DumpContentType { get; set; }
        public DateTime? DumpUpdatedAt { get; set; }
        public bool Processed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? DumpFileSize { get; set; }

        public virtual User? User { get; set; }
    }
}
