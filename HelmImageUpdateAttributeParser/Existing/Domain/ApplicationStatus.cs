using System.Text.Json.Serialization;
using NodaTime;

namespace HelmImageUpdateAttributeParser.Existing.Domain;

public class ApplicationStatus
{
    [JsonPropertyName("sync")]
    public SyncStatus Sync { get; set; } = new();

    [JsonPropertyName("health")]
    public HealthStatus Health { get; set; } = new();

    // Always a list - handles both single sourceType and multiple sourceTypes
    [JsonPropertyName("sourceTypes")] 
    public List<SourceType> SourceTypes { get; set; } = [];

    [JsonPropertyName("summary")]
    public StatusSummary Summary { get; set; } = new();

    [JsonPropertyName("reconciledAt")]
    public Instant ReconciledAt { get; set; } = Instant.MinValue;
}

public class SyncStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class HealthStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class StatusSummary
{
    [JsonPropertyName("images")]
    public List<string> Images { get; set; } = [];
}

// Note: We only support these types currently. Argo offers Kustomize and Plugin as possible types though.
public enum SourceType
{
    Directory,
    Helm
}
