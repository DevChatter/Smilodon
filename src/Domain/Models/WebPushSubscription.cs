namespace Smilodon.Domain.Models
{
    public class WebPushSubscription
    {
        public long Id { get; set; }
        public string Endpoint { get; set; } = null!;
        public string KeyP256dh { get; set; } = null!;
        public string KeyAuth { get; set; } = null!;
        public string? Data { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? AccessTokenId { get; set; }
        public long? UserId { get; set; }

        public virtual OAuthAccessToken? AccessToken { get; set; }
        public virtual User? User { get; set; }
    }
}
