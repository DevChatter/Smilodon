namespace Smilodon.WebApp.Models;

public record PublicKey(string id, string owner, string publicKeyPem);

public record Actor(
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
    PublicKey publicKey) : BaseObject(context, type, id, name);