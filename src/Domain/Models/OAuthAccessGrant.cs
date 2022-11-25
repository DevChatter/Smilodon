namespace Smilodon.Domain.Models
{
    public class OAuthAccessGrant
    {
        public long Id { get; set; }
        public string Token { get; set; } = null!;
        public int ExpiresIn { get; set; }
        public string RedirectUri { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? Scopes { get; set; }
        public long ApplicationId { get; set; }
        public long ResourceOwnerId { get; set; }

        public virtual OAuthApplication Application { get; set; } = null!;
        public virtual User ResourceOwner { get; set; } = null!;
    }
}
