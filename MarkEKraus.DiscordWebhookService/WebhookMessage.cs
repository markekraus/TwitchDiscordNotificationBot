using System.Text.Json.Serialization;

namespace MarkEKraus.DiscordWebhookService
{
    public class WebhookMessage : IWebhookMessage
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        public WebhookMessage(){}
    }
}