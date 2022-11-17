using System.Text.Json.Serialization;

namespace Smilodon.WebApp.Models;

public abstract record BaseObject(
    [property:JsonPropertyName("@context")]
    List<string> context,
    string type,
    string id,
    string name);

public record Collection(
    List<string> context,
    string type,
    string id,
    string name,
    uint totalItems,
    string current, // NOTE: CollectionPage or Link
    string first, // NOTE: CollectionPage or Link
    string last, // NOTE: CollectionPage or Link
    string[] items // NOTE: Each is BaseObject or Link
    ) : BaseObject(context, type, id, name);


public record OrderedCollection(
    List<string> context,
    string type,
    string id,
    string name,
    uint totalItems,
    string current, // NOTE: CollectionPage or Link
    string first, // NOTE: CollectionPage or Link
    string last, // NOTE: CollectionPage or Link
    string[] orderedItems // NOTE: Each is BaseObject or Link
    ) : Collection(context, type, id, name, totalItems, current, first, last, orderedItems);
