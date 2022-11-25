namespace Smilodon.Domain.Models
{
    public class EncryptedMessage
    {
        public long Id { get; set; }
        public long? DeviceId { get; set; }
        public long? FromAccountId { get; set; }
        public string FromDeviceId { get; set; } = null!;
        public int Type { get; set; }
        public string Body { get; set; } = null!;
        public string Digest { get; set; } = null!;
        public string MessageFranking { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Device? Device { get; set; }
        public virtual Account? FromAccount { get; set; }
    }
}
