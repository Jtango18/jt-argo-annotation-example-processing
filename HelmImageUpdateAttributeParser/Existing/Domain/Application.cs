using System.Text.Json.Serialization;
using HelmImageUpdateAttributeParser.Existing.Domain.Converters;


namespace HelmImageUpdateAttributeParser.Existing.Domain;

public class Application
{
    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; } = new();

    [JsonPropertyName("spec")]
    [JsonConverter(typeof(SourcePropertyConverter))]
    public ApplicationSpec Spec { get; set; } = new();

    [JsonPropertyName("status")]
    [JsonConverter(typeof(ApplicationStatusConverter))]
    public ApplicationStatus Status { get; set; } = new();
        
}
