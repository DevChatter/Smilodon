using System.Net;

namespace Smilodon.Domain.Models
{
    public class UserIp
    {
        public long? UserId { get; set; }
        public IPAddress? Ip { get; set; }
        public DateTime? UsedAt { get; set; }
    }
}
