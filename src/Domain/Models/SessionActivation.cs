using System.Net;

namespace Smilodon.Domain.Models
{
    public class SessionActivation
    {
        public long Id { get; set; }
        public string SessionId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserAgent { get; set; } = null!;
        public IPAddress? Ip { get; set; }
        public long? AccessTokenId { get; set; }
        public long UserId { get; set; }
        public long? WebPushSubscriptionId { get; set; }

        public virtual OAuthAccessToken? AccessToken { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
