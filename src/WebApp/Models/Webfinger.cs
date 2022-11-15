namespace Smilodon.WebApp.Models;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public record Link
{
    public string rel { get; set; }
    public string type { get; set; }
    public string href { get; set; }
}

public record WebFinger
{
    public string subject { get; set; }
    public List<Link> links { get; set; }
}

