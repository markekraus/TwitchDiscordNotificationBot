using System.Text.Json.Serialization;

namespace MarkEKraus.DiscordWebhookService.Interfaces
{
    public interface IMessageEmbedAuthor
    {
        [JsonPropertyName("name")]
        string Name { get; set; }

        [JsonPropertyName("url")]
        string Url { get; set; }

        [JsonPropertyName("icon_url")]
        string IconUrl { get; set; }

        [JsonPropertyName("proxy_icon_url")]
        string ProxyIconUrl { get; set; }
    }
}