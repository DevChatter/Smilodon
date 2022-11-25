namespace Smilodon.Domain.Models
{
    public class OAuthApplication
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Uid { get; set; } = null!;
        public string Secret { get; set; } = null!;
        public string RedirectUri { get; set; } = null!;
        public string Scopes { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Superapp { get; set; }
        public string? Website { get; set; }
        public string? OwnerType { get; set; }
        public long? OwnerId { get; set; }
        public bool Confidential { get; set; }

        public virtual User? Owner { get; set; }
        public virtual ICollection<OAuthAccessGrant> OAuthAccessGrants { get; set; } = new HashSet<OAuthAccessGrant>();
        public virtual ICollection<OAuthAccessToken> OAuthAccessTokens { get; set; } = new HashSet<OAuthAccessToken>();
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
