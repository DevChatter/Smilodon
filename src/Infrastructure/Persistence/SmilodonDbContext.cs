using Microsoft.EntityFrameworkCore;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence;

/// <summary>
/// The core application DB Context.
/// </summary>
public class SmilodonDbContext : DbContext
{
    /// <summary>
    /// Constructs a new Smilodon DB Context.
    /// </summary>
    /// <param name="options">The <see cref="DbContextOptions{TContext}"/>.</param>
    public SmilodonDbContext(DbContextOptions<SmilodonDbContext> options) : base(options) { }

    /// <inheritdoc cref="DbContext"/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Required for scripts
        modelBuilder.HasPostgresExtension("plpgsql");
            
        // Built-in sequences
        modelBuilder.HasSequence("accounts_id_seq");
        modelBuilder.HasSequence("encrypted_messages_id_seq");
        modelBuilder.HasSequence("media_attachments_id_seq");
        modelBuilder.HasSequence("statuses_id_seq");
        
        // Apply entity configurations for the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmilodonDbContext).Assembly);
    }

    public virtual DbSet<Account> Accounts { get; set; } = null!;
    public virtual DbSet<AccountAlias> AccountAliases { get; set; } = null!;
    public virtual DbSet<AccountConversation> AccountConversations { get; set; } = null!;
    public virtual DbSet<AccountDeletionRequest> AccountDeletionRequests { get; set; } = null!;
    public virtual DbSet<AccountDomainBlock> AccountDomainBlocks { get; set; } = null!;
    public virtual DbSet<AccountMigration> AccountMigrations { get; set; } = null!;
    public virtual DbSet<AccountModerationNote> AccountModerationNotes { get; set; } = null!;
    public virtual DbSet<AccountNote> AccountNotes { get; set; } = null!;
    public virtual DbSet<AccountPin> AccountPins { get; set; } = null!;
    public virtual DbSet<AccountStat> AccountStats { get; set; } = null!;
    public virtual DbSet<AccountStatusesCleanupPolicy> AccountStatusesCleanupPolicies { get; set; } = null!;
    public virtual DbSet<AccountSummary> AccountSummaries { get; set; } = null!;
    public virtual DbSet<AccountWarning> AccountWarnings { get; set; } = null!;
    public virtual DbSet<AccountWarningPreset> AccountWarningPresets { get; set; } = null!;
    public virtual DbSet<AccountTag> AccountTags { get; set; } = null!;
    public virtual DbSet<AdminActionLog> AdminActionLogs { get; set; } = null!;
    public virtual DbSet<Announcement> Announcements { get; set; } = null!;
    public virtual DbSet<AnnouncementMute> AnnouncementMutes { get; set; } = null!;
    public virtual DbSet<AnnouncementReaction> AnnouncementReactions { get; set; } = null!;
    public virtual DbSet<Appeal> Appeals { get; set; } = null!;
    public virtual DbSet<ArInternalMetadatum> ArInternalMetadata { get; set; } = null!;
    public virtual DbSet<Backup> Backups { get; set; } = null!;
    public virtual DbSet<Block> Blocks { get; set; } = null!;
    public virtual DbSet<Bookmark> Bookmarks { get; set; } = null!;
    public virtual DbSet<CanonicalEmailBlock> CanonicalEmailBlocks { get; set; } = null!;
    public virtual DbSet<Conversation> Conversations { get; set; } = null!;
    public virtual DbSet<ConversationMute> ConversationMutes { get; set; } = null!;
    public virtual DbSet<CustomEmoji> CustomEmojis { get; set; } = null!;
    public virtual DbSet<CustomEmojiCategory> CustomEmojiCategories { get; set; } = null!;
    public virtual DbSet<CustomFilter> CustomFilters { get; set; } = null!;
    public virtual DbSet<CustomFilterKeyword> CustomFilterKeywords { get; set; } = null!;
    public virtual DbSet<CustomFilterStatus> CustomFilterStatuses { get; set; } = null!;
    public virtual DbSet<Device> Devices { get; set; } = null!;
    public virtual DbSet<DomainAllow> DomainAllows { get; set; } = null!;
    public virtual DbSet<DomainBlock> DomainBlocks { get; set; } = null!;
    public virtual DbSet<EmailDomainBlock> EmailDomainBlocks { get; set; } = null!;
    public virtual DbSet<EncryptedMessage> EncryptedMessages { get; set; } = null!;
    public virtual DbSet<Favourite> Favourites { get; set; } = null!;
    public virtual DbSet<FeaturedTag> FeaturedTags { get; set; } = null!;
    public virtual DbSet<Follow> Follows { get; set; } = null!;
    public virtual DbSet<FollowRecommendation> FollowRecommendations { get; set; } = null!;
    public virtual DbSet<FollowRecommendationSuppression> FollowRecommendationSuppressions { get; set; } = null!;
    public virtual DbSet<FollowRequest> FollowRequests { get; set; } = null!;
    public virtual DbSet<Identity> Identities { get; set; } = null!;
    public virtual DbSet<Import> Imports { get; set; } = null!;
    public virtual DbSet<Instance> Instances { get; set; } = null!;
    public virtual DbSet<Invite> Invites { get; set; } = null!;
    public virtual DbSet<IpBlock> IpBlocks { get; set; } = null!;
    public virtual DbSet<List> Lists { get; set; } = null!;
    public virtual DbSet<ListAccount> ListAccounts { get; set; } = null!;
    public virtual DbSet<LoginActivity> LoginActivities { get; set; } = null!;
    public virtual DbSet<Marker> Markers { get; set; } = null!;
    public virtual DbSet<MediaAttachment> MediaAttachments { get; set; } = null!;
    public virtual DbSet<Mention> Mentions { get; set; } = null!;
    public virtual DbSet<Mute> Mutes { get; set; } = null!;
    public virtual DbSet<Notification> Notifications { get; set; } = null!;
    public virtual DbSet<OAuthAccessGrant> OAuthAccessGrants { get; set; } = null!;
    public virtual DbSet<OAuthAccessToken> OAuthAccessTokens { get; set; } = null!;
    public virtual DbSet<OAuthApplication> OAuthApplications { get; set; } = null!;
    public virtual DbSet<OneTimeKey> OneTimeKeys { get; set; } = null!;
    public virtual DbSet<PgheroSpaceStat> PgheroSpaceStats { get; set; } = null!;
    public virtual DbSet<Poll> Polls { get; set; } = null!;
    public virtual DbSet<PollVote> PollVotes { get; set; } = null!;
    public virtual DbSet<PreviewCard> PreviewCards { get; set; } = null!;
    public virtual DbSet<PreviewCardProvider> PreviewCardProviders { get; set; } = null!;
    public virtual DbSet<PreviewCardTrend> PreviewCardTrends { get; set; } = null!;
    public virtual DbSet<PreviewCardStatus> PreviewCardStatuses { get; set; } = null!;
    public virtual DbSet<Relay> Relays { get; set; } = null!;
    public virtual DbSet<Report> Reports { get; set; } = null!;
    public virtual DbSet<ReportNote> ReportNotes { get; set; } = null!;
    public virtual DbSet<Rule> Rules { get; set; } = null!;
    public virtual DbSet<ScheduledStatus> ScheduledStatuses { get; set; } = null!;
    public virtual DbSet<SessionActivation> SessionActivations { get; set; } = null!;
    public virtual DbSet<Setting> Settings { get; set; } = null!;
    public virtual DbSet<SiteUpload> SiteUploads { get; set; } = null!;
    public virtual DbSet<Status> Statuses { get; set; } = null!;
    public virtual DbSet<StatusEdit> StatusEdits { get; set; } = null!;
    public virtual DbSet<StatusPin> StatusPins { get; set; } = null!;
    public virtual DbSet<StatusStat> StatusStats { get; set; } = null!;
    public virtual DbSet<StatusTrend> StatusTrends { get; set; } = null!;
    public virtual DbSet<StatusTag> StatusesTags { get; set; } = null!;
    public virtual DbSet<SystemKey> SystemKeys { get; set; } = null!;
    public virtual DbSet<Tag> Tags { get; set; } = null!;
    public virtual DbSet<TagFollow> TagFollows { get; set; } = null!;
    public virtual DbSet<Tombstone> Tombstones { get; set; } = null!;
    public virtual DbSet<UnavailableDomain> UnavailableDomains { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<UserInviteRequest> UserInviteRequests { get; set; } = null!;
    public virtual DbSet<UserIp> UserIps { get; set; } = null!;
    public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
    public virtual DbSet<WebPushSubscription> WebPushSubscriptions { get; set; } = null!;
    public virtual DbSet<WebSetting> WebSettings { get; set; } = null!;
    public virtual DbSet<WebAuthnCredential> WebAuthnCredentials { get; set; } = null!;
    public virtual DbSet<Webhook> Webhooks { get; set; } = null!;
}