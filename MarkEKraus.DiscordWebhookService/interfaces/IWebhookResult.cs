using System.Net.Http;

namespace MarkEKraus.DiscordWebhookService.Interfaces
{
    public interface IWebhookResult
    {
        WebhookOptions WebhookOptions { get; set; }
        IWebhookMessage WebhookMessage { get; set; }
        HttpResponseMessage HttpResponseMessage { get; set; }

        string ResponseBody { get; set; }
        bool IsSuccess { get; set; }
    }
}