using System.Net;

namespace Smilodon.Domain.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string EncryptedPassword { get; set; } = null!;
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordSentAt { get; set; }
        public int SignInCount { get; set; }
        public DateTime? CurrentSignInAt { get; set; }
        public DateTime? LastSignInAt { get; set; }
        public bool Admin { get; set; }
        public string? ConfirmationToken { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ConfirmationSentAt { get; set; }
        public string? UnconfirmedEmail { get; set; }
        public string? Locale { get; set; }
        public string? EncryptedOtpSecret { get; set; }
        public string? EncryptedOtpSecretIv { get; set; }
        public string? EncryptedOtpSecretSalt { get; set; }
        public int? ConsumedTimestep { get; set; }
        public bool OtpRequiredForLogin { get; set; }
        public DateTime? LastEmailedAt { get; set; }
        public string[]? OtpBackupCodes { get; set; }
        public long AccountId { get; set; }
        public bool Disabled { get; set; }
        public bool Moderator { get; set; }
        public long? InviteId { get; set; }
        public string[]? ChosenLanguages { get; set; }
        public long? CreatedByApplicationId { get; set; }
        public bool Approved { get; set; }
        public string? SignInToken { get; set; }
        public DateTime? SignInTokenSentAt { get; set; }
        public string? WebAuthnId { get; set; }
        public IPAddress? SignUpIp { get; set; }
        public bool? SkipSignInToken { get; set; }
        public long? RoleId { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual OAuthApplication? CreatedByApplication { get; set; }
        public virtual Invite? Invite { get; set; }
        public virtual UserRole? Role { get; set; }
        public virtual WebSetting WebSetting { get; set; } = null!;
        public virtual ICollection<Backup> Backups { get; set; } = new HashSet<Backup>();
        public virtual ICollection<Identity> Identities { get; set; } = new HashSet<Identity>();
        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
        public virtual ICollection<LoginActivity> LoginActivities { get; set; } = new HashSet<LoginActivity>();
        public virtual ICollection<Marker> Markers { get; set; } = new HashSet<Marker>();
        public virtual ICollection<OAuthAccessGrant> OAuthAccessGrants { get; set; } = new HashSet<OAuthAccessGrant>();
        public virtual ICollection<OAuthAccessToken> OAuthAccessTokens { get; set; } = new HashSet<OAuthAccessToken>();
        public virtual ICollection<OAuthApplication> OAuthApplications { get; set; } = new HashSet<OAuthApplication>();
        public virtual ICollection<SessionActivation> SessionActivations { get; set; } = new HashSet<SessionActivation>();
        public virtual ICollection<UserInviteRequest> UserInviteRequests { get; set; } = new HashSet<UserInviteRequest>();
        public virtual ICollection<WebPushSubscription> WebPushSubscriptions { get; set; } = new HashSet<WebPushSubscription>();
        public virtual ICollection<WebAuthnCredential> WebAuthnCredentials { get; set; } = new HashSet<WebAuthnCredential>();
    }
}
