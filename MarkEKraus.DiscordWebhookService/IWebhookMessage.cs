using System.Text.Json.Serialization;

namespace MarkEKraus.DiscordWebhookService
{
    public interface IWebhookMessage
    {
        [JsonPropertyName("content")]
        string Content { get; set; }
    }
}