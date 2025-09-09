using System.Text.Json.Serialization;


namespace HelmImageUpdateAttributeParser.Existing.Domain;

public class ApplicationSpec
{
    [JsonPropertyName("destination")]
    public Destination Destination { get; set; } = new();

    [JsonPropertyName("project")]
    public string Project { get; set; } = string.Empty;

    // Always a list - handles both single source and multiple sources
    public List<SourceBase> Sources { get; set; } = [];
}
