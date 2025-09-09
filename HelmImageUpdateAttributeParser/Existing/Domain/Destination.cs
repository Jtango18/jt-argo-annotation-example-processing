using System.Text.Json.Serialization;

namespace HelmImageUpdateAttributeParser.Existing.Domain;

public class Destination
{
    [JsonPropertyName("server")]
    public string Server { get; set; } = string.Empty;

    [JsonPropertyName("namespace")]
    public string Namespace { get; set; } = string.Empty;
}
