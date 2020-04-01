using System.Text.Json.Serialization;

namespace MarkEKraus.DiscordWebhookService.Interfaces
{
    public interface IMessageEmbedProvider
    {
        [JsonPropertyName("name")]
        string Name { get; set; }

        [JsonPropertyName("url")]
        string Url { get; set; }
    }
}