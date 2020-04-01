using System.Text.Json.Serialization;
using MarkEKraus.DiscordWebhookService.Interfaces;

namespace MarkEKraus.DiscordWebhookService.Models
{
    public class MessageEmbedField : IMessageEmbedField
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("inline")]
        public bool IsInline { get; set; }

        public MessageEmbedField(){}
    }
}