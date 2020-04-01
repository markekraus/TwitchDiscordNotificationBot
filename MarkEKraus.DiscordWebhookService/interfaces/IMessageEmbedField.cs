using System.Text.Json.Serialization;

namespace MarkEKraus.DiscordWebhookService.Interfaces
{
    public interface IMessageEmbedField
    {
        [JsonPropertyName("name")]
        string Name { get; set; }

        [JsonPropertyName("value")]
        string Value { get; set; }

        [JsonPropertyName("inline")]
        bool IsInline { get; set; }
    }
}