namespace Smilodon.Domain.Models
{
    public class SystemKey
    {
        public long Id { get; set; }
        public byte[]? Key { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
