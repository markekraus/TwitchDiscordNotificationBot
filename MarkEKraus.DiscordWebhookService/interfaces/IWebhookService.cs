using System.Threading.Tasks;

namespace MarkEKraus.DiscordWebhookService.Interfaces
{
    public interface IWebhookService
    {
        Task<IWebhookResult> SendMessageAsync(IWebhookMessage Message);
    }
}