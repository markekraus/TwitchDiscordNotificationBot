using System.Text.Json.Serialization;

namespace MarkEKraus.DiscordWebhookService.Interfaces
{
    public interface IMessageEmbedVideo
    {
        [JsonPropertyName("url")]
        string Url { get; set; }

        [JsonPropertyName("height")]
        int Height { get; set; }

        [JsonPropertyName("width")]
        int Width { get; set; }
    }
}