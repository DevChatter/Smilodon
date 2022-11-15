namespace Smilodon.WebApp.Models;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class PublicKey
{
    public string id { get; set; }
    public string owner { get; set; }
    public string publicKeyPem { get; set; }
}

public class Actor
{
    [JsonProperty("@context")]
    public List<string> context { get; set; }
    public string id { get; set; }
    public string type { get; set; }
    public string preferredUsername { get; set; }
    public string inbox { get; set; }
    public PublicKey publicKey { get; set; }
}

