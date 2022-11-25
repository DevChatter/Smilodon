using ActivityPub.Domain.Emojis;

namespace ActivityPub.Domain.Accounts;

public class Account
{
    public Account(
        string id, 
        string username, 
        string accountUri,
        string url, 
        string displayName, 
        string note, 
        string avatar,
        string avatarStatic, 
        string headerImage, 
        string headerStatic,
        bool isLocked, 
        ProfileField[] fields, 
        Emoji[] emojis, 
        bool isBot,
        bool isGroup, 
        bool? isDiscoverable, 
        Account hasMovedTo, 
        bool? isSuspended,
        bool isLimited, 
        DateTime createdAt, 
        DateTime? lastStatusAt,
        int statusCount, 
        int followerCount, 
        int followingCount)
    {
        Id = id;
        Username = username;
        AccountUri = accountUri;
        Url = url;
        DisplayName = displayName;
        Note = note;
        Avatar = avatar;
        AvatarStatic = avatarStatic;
        HeaderImage = headerImage;
        HeaderStatic = headerStatic;
        IsLocked = isLocked;
        Fields = fields;
        Emojis = emojis;
        IsBot = isBot;
        IsGroup = isGroup;
        IsDiscoverable = isDiscoverable;
        HasMovedTo = hasMovedTo;
        IsSuspended = isSuspended;
        IsLimited = isLimited;
        CreatedAt = createdAt;
        LastStatusAt = lastStatusAt;
        StatusCount = statusCount;
        FollowerCount = followerCount;
        FollowingCount = followingCount;
    }

    /// <summary>
    /// The Account Id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The Username, not including domain.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Same as Username for local, or Username@Domain for remote.
    /// </summary>
    public string AccountUri { get; set; }

    /// <summary>
    /// The location of the user’s profile page.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The profile’s display name.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// The profile’s bio or description (HTML).
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// URL to the Avatar image for the account.
    /// Shown on profile and next to posts.
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// URL to a static version of the Avatar.
    /// Equal to Avatar if its value is a static image;
    /// different if Avatar is an animated GIF.
    /// </summary>
    public string AvatarStatic { get; set; }

    /// <summary>
    /// URL to a banner image shown above the profile.
    /// </summary>
    public string HeaderImage { get; set; }

    /// <summary>
    /// URL to a static version of the Header.
    /// Equal to Header if its value is a static image;
    /// different if Header is an animated GIF.
    /// </summary>
    public string HeaderStatic { get; set; }

    /// <summary>
    /// Whether the account manually approves follow requests.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Additional metadata attached to a profile as name-value pairs.
    /// </summary>
    public ProfileField[] Fields { get; set; }

    /// <summary>
    /// Custom Emojis to be used when rendering the profile.
    /// </summary>
    public Emoji[] Emojis { get; set; }

    /// <summary>
    /// Indicates that the account may perform automated actions,
    /// may not be monitored, or identifies as a robot.
    /// </summary>
    public bool IsBot { get; set; }

    /// <summary>
    /// Indicates that the account represents a Group actor.
    /// </summary>
    public bool IsGroup { get; set; }

    /// <summary>
    /// Whether the account has opted into discovery features
    /// such as the profile directory.
    /// </summary>
    public bool? IsDiscoverable { get; set; }

    /// <summary>
    /// Indicates that the profile is currently inactive
    /// and that its user has moved to a new account.
    /// </summary>
    public Account HasMovedTo { get; set; }

    /// <summary>
    /// An extra attribute returned only when an account is suspended.
    /// </summary>
    public bool? IsSuspended { get; set; }

    /// <summary>
    /// An extra attribute returned only when an account is silenced.
    /// Indicates that the account should be hidden behind a warning screen.
    /// </summary>
    public bool IsLimited { get; set; }

    /// <summary>
    /// When the account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the most recent status was posted.
    /// </summary>
    public DateTime? LastStatusAt { get; set; }

    /// <summary>
    /// How many statuses are attached to this account.
    /// </summary>
    public int StatusCount { get; set; }

    /// <summary>
    /// The reported followers of this profile.
    /// </summary>
    public int FollowerCount { get; set; }

    /// <summary>
    /// The reported follows of this profile.
    /// </summary>
    public int FollowingCount { get; set; }
}
