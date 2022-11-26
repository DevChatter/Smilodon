using System.Net;

namespace Smilodon.Domain.Models
{
    public class IpBlock
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public IPAddress Ip { get; set; } = null!;
        public int Severity { get; set; }
        public string Comment { get; set; } = null!;
    }
}
