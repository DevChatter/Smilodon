namespace Smilodon.WebApp.Models;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public record Link(string rel, string type, string href);

public record WebFinger(string subject, List<Link> links);
