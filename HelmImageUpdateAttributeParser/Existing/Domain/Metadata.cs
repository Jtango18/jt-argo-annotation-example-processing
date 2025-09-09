using System.Text.Json.Serialization;

namespace HelmImageUpdateAttributeParser.Existing.Domain;

public class Metadata
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("uid")]
    public Guid Uid { get; set; } = Guid.Empty;

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = string.Empty;

    [JsonPropertyName("annotations")]
    public Dictionary<string, string> Annotations { get; set; } = [];
}
