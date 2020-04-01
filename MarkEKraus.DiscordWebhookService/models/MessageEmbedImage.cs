using System.Text.Json.Serialization;
using MarkEKraus.DiscordWebhookService.Interfaces;

namespace MarkEKraus.DiscordWebhookService.Models
{
    public class MessageEmbedImage : IMessageEmbedImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        public MessageEmbedImage(){}
    }
}