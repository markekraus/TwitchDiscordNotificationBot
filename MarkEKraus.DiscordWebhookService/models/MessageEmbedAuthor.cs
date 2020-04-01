using System.Text.Json.Serialization;
using MarkEKraus.DiscordWebhookService.Interfaces;

namespace MarkEKraus.DiscordWebhookService.Models
{
    public class MessageEmbedAuthor : IMessageEmbedAuthor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }

        [JsonPropertyName("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }

        public MessageEmbedAuthor(){}
    }
}