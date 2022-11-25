namespace Smilodon.Domain.Models
{
    public class Account
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Domain { get; set; }
        public string? PrivateKey { get; set; }
        public string PublicKey { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Note { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Uri { get; set; } = null!;
        public string? Url { get; set; }
        public string? AvatarFileName { get; set; }
        public string? AvatarContentType { get; set; }
        public int? AvatarFileSize { get; set; }
        public DateTime? AvatarUpdatedAt { get; set; }
        public string? HeaderFileName { get; set; }
        public string? HeaderContentType { get; set; }
        public int? HeaderFileSize { get; set; }
        public DateTime? HeaderUpdatedAt { get; set; }
        public string? AvatarRemoteUrl { get; set; }
        public bool Locked { get; set; }
        public string HeaderRemoteUrl { get; set; } = null!;
        public DateTime? LastWebfingeredAt { get; set; }
        public string InboxUrl { get; set; } = null!;
        public string OutboxUrl { get; set; } = null!;
        public string SharedInboxUrl { get; set; } = null!;
        public string FollowersUrl { get; set; } = null!;
        public int Protocol { get; set; }
        public bool Memorial { get; set; }
        public long? MovedToAccountId { get; set; }
        public string? FeaturedCollectionUrl { get; set; }
        public string? Fields { get; set; }
        public string? ActorType { get; set; }
        public bool? Discoverable { get; set; }
        public string[]? AlsoKnownAs { get; set; }
        public DateTime? SilencedAt { get; set; }
        public DateTime? SuspendedAt { get; set; }
        public bool? HideCollections { get; set; }
        public int? AvatarStorageSchemaVersion { get; set; }
        public int? HeaderStorageSchemaVersion { get; set; }
        public string? DevicesUrl { get; set; }
        public int? SuspensionOrigin { get; set; }
        public DateTime? SensitizedAt { get; set; }
        public bool? Trendable { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime? RequestedReviewAt { get; set; }

        public virtual Account? MovedToAccount { get; set; }
        public virtual AccountStat AccountStat { get; set; } = null!;
        public virtual FollowRecommendationSuppression FollowRecommendationSuppression { get; set; } = null!;
        public virtual ICollection<AccountAlias> AccountAliases { get; set; } = new HashSet<AccountAlias>();
        public virtual ICollection<AccountConversation> AccountConversations { get; set; } = new HashSet<AccountConversation>();
        public virtual ICollection<AccountDeletionRequest> AccountDeletionRequests { get; set; } = new HashSet<AccountDeletionRequest>();
        public virtual ICollection<AccountDomainBlock> AccountDomainBlocks { get; set; } = new HashSet<AccountDomainBlock>();
        public virtual ICollection<AccountMigration> AccountMigrationAccounts { get; set; } = new HashSet<AccountMigration>();
        public virtual ICollection<AccountMigration> AccountMigrationTargetAccounts { get; set; } = new HashSet<AccountMigration>();
        public virtual ICollection<AccountModerationNote> AccountModerationNoteAccounts { get; set; } = new HashSet<AccountModerationNote>();
        public virtual ICollection<AccountModerationNote> AccountModerationNoteTargetAccounts { get; set; } = new HashSet<AccountModerationNote>();
        public virtual ICollection<AccountNote> AccountNoteAccounts { get; set; } = new HashSet<AccountNote>();
        public virtual ICollection<AccountNote> AccountNoteTargetAccounts { get; set; } = new HashSet<AccountNote>();
        public virtual ICollection<AccountPin> AccountPinAccounts { get; set; } = new HashSet<AccountPin>();
        public virtual ICollection<AccountPin> AccountPinTargetAccounts { get; set; } = new HashSet<AccountPin>();
        public virtual ICollection<AccountStatusesCleanupPolicy> AccountStatusesCleanupPolicies { get; set; } = new HashSet<AccountStatusesCleanupPolicy>();
        public virtual ICollection<AccountWarning> AccountWarningAccounts { get; set; } = new HashSet<AccountWarning>();
        public virtual ICollection<AccountWarning> AccountWarningTargetAccounts { get; set; } = new HashSet<AccountWarning>();
        public virtual ICollection<AdminActionLog> AdminActionLogs { get; set; } = new HashSet<AdminActionLog>();
        public virtual ICollection<AnnouncementMute> AnnouncementMutes { get; set; } = new HashSet<AnnouncementMute>();
        public virtual ICollection<AnnouncementReaction> AnnouncementReactions { get; set; } = new HashSet<AnnouncementReaction>();
        public virtual ICollection<Appeal> AppealAccounts { get; set; } = new HashSet<Appeal>();
        public virtual ICollection<Appeal> AppealApprovedByAccounts { get; set; } = new HashSet<Appeal>();
        public virtual ICollection<Appeal> AppealRejectedByAccounts { get; set; } = new HashSet<Appeal>();
        public virtual ICollection<Block> BlockAccounts { get; set; } = new HashSet<Block>();
        public virtual ICollection<Block> BlockTargetAccounts { get; set; } = new HashSet<Block>();
        public virtual ICollection<Bookmark> Bookmarks { get; set; } = new HashSet<Bookmark>();
        public virtual ICollection<CanonicalEmailBlock> CanonicalEmailBlocks { get; set; } = new HashSet<CanonicalEmailBlock>();
        public virtual ICollection<ConversationMute> ConversationMutes { get; set; } = new HashSet<ConversationMute>();
        public virtual ICollection<CustomFilter> CustomFilters { get; set; } = new HashSet<CustomFilter>();
        public virtual ICollection<Device> Devices { get; set; } = new HashSet<Device>();
        public virtual ICollection<EncryptedMessage> EncryptedMessages { get; set; } = new HashSet<EncryptedMessage>();
        public virtual ICollection<Favourite> Favourites { get; set; } = new HashSet<Favourite>();
        public virtual ICollection<FeaturedTag> FeaturedTags { get; set; } = new HashSet<FeaturedTag>();
        public virtual ICollection<Follow> FollowAccounts { get; set; } = new HashSet<Follow>();
        public virtual ICollection<FollowRequest> FollowRequestAccounts { get; set; } = new HashSet<FollowRequest>();
        public virtual ICollection<FollowRequest> FollowRequestTargetAccounts { get; set; } = new HashSet<FollowRequest>();
        public virtual ICollection<Follow> FollowTargetAccounts { get; set; } = new HashSet<Follow>();
        public virtual ICollection<Import> Imports { get; set; } = new HashSet<Import>();
        public virtual ICollection<Account> InverseMovedToAccount { get; set; } = new HashSet<Account>();
        public virtual ICollection<ListAccount> ListAccounts { get; set; } = new HashSet<ListAccount>();
        public virtual ICollection<List> Lists { get; set; } = new HashSet<List>();
        public virtual ICollection<MediaAttachment> MediaAttachments { get; set; } = new HashSet<MediaAttachment>();
        public virtual ICollection<Mention> Mentions { get; set; } = new HashSet<Mention>();
        public virtual ICollection<Mute> MuteAccounts { get; set; } = new HashSet<Mute>();
        public virtual ICollection<Mute> MuteTargetAccounts { get; set; } = new HashSet<Mute>();
        public virtual ICollection<Notification> NotificationAccounts { get; set; } = new HashSet<Notification>();
        public virtual ICollection<Notification> NotificationFromAccounts { get; set; } = new HashSet<Notification>();
        public virtual ICollection<PollVote> PollVotes { get; set; } = new HashSet<PollVote>();
        public virtual ICollection<Poll> Polls { get; set; } = new HashSet<Poll>();
        public virtual ICollection<Report> ReportAccounts { get; set; } = new HashSet<Report>();
        public virtual ICollection<Report> ReportActionTakenByAccounts { get; set; } = new HashSet<Report>();
        public virtual ICollection<Report> ReportAssignedAccounts { get; set; } = new HashSet<Report>();
        public virtual ICollection<ReportNote> ReportNotes { get; set; } = new HashSet<ReportNote>();
        public virtual ICollection<Report> ReportTargetAccounts { get; set; } = new HashSet<Report>();
        public virtual ICollection<ScheduledStatus> ScheduledStatuses { get; set; } = new HashSet<ScheduledStatus>();
        public virtual ICollection<Status> StatusAccounts { get; set; } = new HashSet<Status>();
        public virtual ICollection<StatusEdit> StatusEdits { get; set; } = new HashSet<StatusEdit>();
        public virtual ICollection<Status> StatusInReplyToAccounts { get; set; } = new HashSet<Status>();
        public virtual ICollection<StatusPin> StatusPins { get; set; } = new HashSet<StatusPin>();
        public virtual ICollection<StatusTrend> StatusTrends { get; set; } = new HashSet<StatusTrend>();
        public virtual ICollection<TagFollow> TagFollows { get; set; } = new HashSet<TagFollow>();
        public virtual ICollection<Tombstone> Tombstones { get; set; } = new HashSet<Tombstone>();
        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
