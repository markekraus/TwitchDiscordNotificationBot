using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MarkEKraus.DiscordWebhookService.Models;

namespace MarkEKraus.DiscordWebhookService.Interfaces
{
    public interface IMessageEmbed
    {
        [JsonPropertyName("title")]
        string Title { get; set; }
        
        [JsonPropertyName("type")]
        MessageEmbedType Type { get; set; }
        
        [JsonPropertyName("description")]
        string Description { get; set; }
        
        [JsonPropertyName("url")]
        string Url { get; set; }
        
        [JsonPropertyName("timestamp")]
        DateTime Timestamp { get; set; }
        
        [JsonPropertyName("color")]
        int Color { get; set; }

        [JsonPropertyName("footer")]
        IMessageEmbedFooter Footer { get; set; }

        [JsonPropertyName("image")]
        IMessageEmbedImage Image { get; set; }

        [JsonPropertyName("thumbnail")]
        IMessageEmbedImage Thumbnail { get; set; }

        [JsonPropertyName("video")]
        IMessageEmbedVideo Video { get; set; }

        [JsonPropertyName("provider")]
        IMessageEmbedProvider Provider { get; set; }

        [JsonPropertyName("author")]
        IMessageEmbedAuthor Author { get; set; }

        [JsonPropertyName("fields")]
        IList<IMessageEmbedField> Fields { get; set; }
    }
}