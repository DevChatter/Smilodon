using System.Text.Json.Serialization;

namespace Smilodon.WebApp.Models;

public record PublicKey
{
    public string id { get; set; }
    public string owner { get; set; }
    public string publicKeyPem { get; set; }
}

public record Actor(
    [property: JsonPropertyName("@context")]
    List<string> context,
    string type,
    string id,
    string following,
    string followers,
    string liked,
    string inbox,
    string outbox,
    string preferredUsername,
    string name,
    string summary,
    string[] icon,
    PublicKey publicKey)
        : BaseObject(context, type, id, name)
{
}

