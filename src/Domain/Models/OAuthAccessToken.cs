using System.Net;

namespace Smilodon.Domain.Models
{
    public class OAuthAccessToken
    {
        public long Id { get; set; }
        public string Token { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public int? ExpiresIn { get; set; }
        public DateTime? RevokedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Scopes { get; set; }
        public long? ApplicationId { get; set; }
        public long? ResourceOwnerId { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public IPAddress? LastUsedIp { get; set; }

        public virtual OAuthApplication? Application { get; set; }
        public virtual User? ResourceOwner { get; set; }
        public virtual ICollection<Device> Devices { get; set; } = new HashSet<Device>();
        public virtual ICollection<SessionActivation> SessionActivations { get; set; } = new HashSet<SessionActivation>();
        public virtual ICollection<WebPushSubscription> WebPushSubscriptions { get; set; } = new HashSet<WebPushSubscription>();
    }
}
