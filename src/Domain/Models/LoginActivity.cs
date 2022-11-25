using System.Net;

namespace Smilodon.Domain.Models
{
    public class LoginActivity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? AuthenticationMethod { get; set; }
        public string? Provider { get; set; }
        public bool? Success { get; set; }
        public string? FailureReason { get; set; }
        public IPAddress? Ip { get; set; }
        public string? UserAgent { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
