using System.Text.Json.Serialization;

namespace MarkEKraus.DiscordWebhookService.Interfaces
{
    public interface IMessageEmbedFooter
    {
        [JsonPropertyName("text")]
        string Text { get; set; }

        [JsonPropertyName("icon_url")]
        string IconUrl { get; set; }

        [JsonPropertyName("proxy_icon_url")]
        string ProxyIconUrl { get; set; }
    }
}