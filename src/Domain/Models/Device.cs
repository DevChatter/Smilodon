namespace Smilodon.Domain.Models
{
    public class Device
    {
        public long Id { get; set; }
        public long? AccessTokenId { get; set; }
        public long? AccountId { get; set; }
        public string DeviceId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string FingerprintKey { get; set; } = null!;
        public string IdentityKey { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual OAuthAccessToken? AccessToken { get; set; }
        public virtual Account? Account { get; set; }
        public virtual ICollection<EncryptedMessage> EncryptedMessages { get; set; } = new HashSet<EncryptedMessage>();
        public virtual ICollection<OneTimeKey> OneTimeKeys { get; set; } = new HashSet<OneTimeKey>();
    }
}
