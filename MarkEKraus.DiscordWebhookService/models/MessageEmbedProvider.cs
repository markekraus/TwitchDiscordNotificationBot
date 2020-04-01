using System.Text.Json.Serialization;
using MarkEKraus.DiscordWebhookService.Interfaces;

namespace MarkEKraus.DiscordWebhookService.Models
{
    public class MessageEmbedProvider : IMessageEmbedProvider
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        public MessageEmbedProvider(){}
    }
}