using System.Text.Json.Serialization;

namespace HelmImageUpdateAttributeParser.Existing.Domain;

public class HelmConfig
{
    [JsonPropertyName("valueFiles")]
    public List<string> ValueFiles { get; set; } = [];
}
