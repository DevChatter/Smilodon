namespace ActivityPub.Domain.Emojis;

public class Emoji
{
    public string ShortCode { get; set; }
    public string Url { get; set; }
    public string StaticUrl { get; set; }
    public bool VisibleInPicker { get; set; }
}
