using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;
using System.Net.Mime;

namespace MarkEKraus.DiscordWebhookService
{
    public class WebhookService : IWebhookService
    {
        private HttpClient _httpClient;
        private ILogger<WebhookService> _logger;
        private IOptions<WebhookOptions> _config;

        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

        public WebhookService(
            HttpClient httpClient,
            ILogger<WebhookService> logger,
            IOptions<WebhookOptions> config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config;
        }
        public async Task<IWebhookResult> SendMessageAsync(IWebhookMessage Message)
        {
            var httpMessageBody = JsonSerializer.Serialize(Message, jsonOptions);

            var httpMessage = new HttpRequestMessage()
            {
                RequestUri = _config.Value.WebhookUri,
                Content = new StringContent(httpMessageBody, Encoding.UTF8, MediaTypeNames.Application.Json),
                Method = HttpMethod.Post
            };

            var httpResponse = await _httpClient.SendAsync(httpMessage, HttpCompletionOption.ResponseHeadersRead);

            if(!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogError($"SendMessageAsync Request Failed");
            }

            _logger.LogInformation($"SendMessageAsync Success: {httpResponse.IsSuccessStatusCode}");
            _logger.LogInformation($"SendMessageAsync StatusCode: {httpResponse.StatusCode}");
            _logger.LogInformation($"SendMessageAsync ReasonPhrase: {httpResponse.ReasonPhrase}");
            
            var responseBody = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogInformation($"SendMessageAsync Response:");
            _logger.LogInformation(responseBody);

            return new WebhookResult
            {
                HttpResponseMessage = httpResponse,
                ResponseBody = responseBody,
                WebhookMessage = Message,
                WebhookOptions = _config.Value,
                IsSuccess = httpResponse.IsSuccessStatusCode
            };
        }
    }
}