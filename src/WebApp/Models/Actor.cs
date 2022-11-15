using System.Text.Json;

namespace Smilodon.WebApp.Models;

public record PublicKey
{
    public string id { get; set; }
    public string owner { get; set; }
    public string publicKeyPem { get; set; }
}

public record Actor
{
    [JsonPropertyName("@context")]
    public List<string> context { get; set; }
    public string id { get; set; }
    public string type { get; set; }
    public string preferredUsername { get; set; }
    public string inbox { get; set; }
    public PublicKey publicKey { get; set; }
}

