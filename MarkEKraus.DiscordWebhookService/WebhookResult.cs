using System.Net.Http;

namespace MarkEKraus.DiscordWebhookService
{
    public class WebhookResult : IWebhookResult
    {
        public WebhookOptions WebhookOptions { get; set; }
        public IWebhookMessage WebhookMessage { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public string ResponseBody { get; set; }
        public bool IsSuccess { get; set; }

        public WebhookResult(){}

    }
}