using System.Threading.Tasks;

namespace MarkEKraus.DiscordWebhookService
{
    public interface IWebhookService
    {
        Task<IWebhookResult> SendMessageAsync(IWebhookMessage Message);
    }
}