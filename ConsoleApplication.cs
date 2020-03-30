using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Services;
using System.Linq;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace TwitchDiscordNotificationBot
{
    internal class ConsoleApplication
    {
        private ILogger<ConsoleApplication> _logger;
        private IOptions<AppSettings> _config;
        private TwitchAPI _twitchApi;
        private IApiSettings _apiSettings;
        private LiveStreamMonitorService _twitchMonitor;
        public ConsoleApplication(
            ILogger<ConsoleApplication> logger,
            IOptions<AppSettings> config,
            TwitchAPI twitchApi,
            IApiSettings apiSettings
        )
        {
            _logger = logger;
            _config = config;
            _twitchApi = twitchApi;
            _apiSettings = apiSettings;
            _twitchMonitor = new LiveStreamMonitorService(_twitchApi, config.Value.TwitchApiCheckIntervalSeconds);
        }

        public async Task Run()
        {
            _logger.LogInformation($"{nameof(TwitchDiscordNotificationBot)} Start");
            
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
        }
    }
}