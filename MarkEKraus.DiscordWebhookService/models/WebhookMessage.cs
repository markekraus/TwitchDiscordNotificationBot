using System.Collections.Generic;
using System.Text.Json.Serialization;
using MarkEKraus.DiscordWebhookService.Interfaces;

namespace MarkEKraus.DiscordWebhookService.Models
{
    public class WebhookMessage : IWebhookMessage
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("embeds")]
        public IList<IMessageEmbed> Embeds { get; set; }

        public WebhookMessage(){}
    }
}