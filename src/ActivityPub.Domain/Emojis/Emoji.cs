namespace ActivityPub.Domain.Emojis;

public class Emoji
{
    /// <summary>
    /// The name of the custom Emoji.
    /// </summary>
    public string ShortCode { get; set; }

    /// <summary>
    /// URL to the custom Emoji image.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// URL to the static version of the custom Emoji.
    /// </summary>
    public string StaticUrl { get; set; }

    /// <summary>
    /// Whether this Emoji should be visible in the picker or unlisted.
    /// </summary>
    public bool IsVisibleInPicker { get; set; }

    /// <summary>
    /// Used for grouping custom emojis.
    /// </summary>
    public string Category { get; set; }
}
