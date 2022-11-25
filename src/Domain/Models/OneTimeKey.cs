namespace Smilodon.Domain.Models
{
    public class OneTimeKey
    {
        public long Id { get; set; }
        public long? DeviceId { get; set; }
        public string KeyId { get; set; } = null!;
        public string Key { get; set; } = null!;
        public string Signature { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Device? Device { get; set; }
    }
}
