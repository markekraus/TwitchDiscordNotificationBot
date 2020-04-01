using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MarkEKraus.DiscordWebhookService.Interfaces
{
    public interface IWebhookMessage
    {
        [JsonPropertyName("content")]
        string Content { get; set; }

        [JsonPropertyName("embeds")]
        IList<IMessageEmbed> Embeds { get; set; }
    }
}