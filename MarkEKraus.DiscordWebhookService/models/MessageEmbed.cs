using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MarkEKraus.DiscordWebhookService.Interfaces;

namespace MarkEKraus.DiscordWebhookService.Models
{
    public class MessageEmbed: IMessageEmbed
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("type")]
        public MessageEmbedType Type { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        
        [JsonPropertyName("color")]
        public int Color { get; set; }

        [JsonPropertyName("footer")]
        public IMessageEmbedFooter Footer { get; set; }

        [JsonPropertyName("image")]
        public IMessageEmbedImage Image { get; set; }

        [JsonPropertyName("thumbnail")]
        public IMessageEmbedImage Thumbnail { get; set; }

        [JsonPropertyName("video")]
        public IMessageEmbedVideo Video { get; set; }

        [JsonPropertyName("provider")]
        public IMessageEmbedProvider Provider { get; set; }

        [JsonPropertyName("author")]
        public IMessageEmbedAuthor Author { get; set; }

        [JsonPropertyName("fields")]
        public IList<IMessageEmbedField> Fields { get; set; }

        public MessageEmbed(){}
    }
}