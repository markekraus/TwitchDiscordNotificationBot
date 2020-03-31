using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Services;
using System.Linq;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Api.Interfaces;
using MarkEKraus.DiscordWebhookService;
using System.Collections.Generic;

namespace MarkEKraus.TwitchDiscordNotificationBot
{
    internal class ConsoleApplication
    {
        private ILogger<ConsoleApplication> _logger;
        private IOptions<AppSettings> _config;
        private ITwitchAPI _twitchApi;
        private IApiSettings _apiSettings;
        private LiveStreamMonitorService _twitchMonitor;
        private IWebhookService _webhookService;

        public ConsoleApplication(
            ILogger<ConsoleApplication> logger,
            IOptions<AppSettings> config,
            ITwitchAPI twitchApi,
            IApiSettings apiSettings,
            IWebhookService webhookService
        )
        {
            _logger = logger;
            _config = config;
            _twitchApi = twitchApi;
            _apiSettings = apiSettings;
            _webhookService = webhookService;
            _twitchMonitor = new LiveStreamMonitorService(_twitchApi, config.Value.TwitchApiCheckIntervalSeconds);
        }

        public async Task Run()
        {
            _logger.LogInformation($"{nameof(MarkEKraus.TwitchDiscordNotificationBot)} Start");
            
            _twitchMonitor.SetChannelsByName(
                _config.Value.TwitchChannels
                .Select(channel => channel.Channel)
                .ToList());
            
            _twitchMonitor.OnStreamOnline += Monitor_OnStreamOnline;

            _twitchMonitor.Start();

            await Task.Delay(-1);
        }

        private void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs args)
        {
            _logger.LogInformation($"channel: {args.Channel}");
            var game = _twitchApi.Helix.Games.GetGamesAsync(new List<string>{args.Stream.GameId}).GetAwaiter().GetResult();
            _webhookService.SendMessageAsync(new WebhookMessage{Content = $"{args.Stream.UserName} is streaming {game.Games.FirstOrDefault().Name} https://twitch.tv/{args.Channel}"});
        }
    }
}