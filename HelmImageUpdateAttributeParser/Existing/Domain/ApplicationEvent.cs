using System.Text.Json.Serialization;

namespace HelmImageUpdateAttributeParser.Existing.Domain
{
    public class ApplicationEvent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("application")]
        public Application Application { get; set; } = new();
    }
}
