using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Services;
using System.Linq;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Api.Interfaces;
using System.Collections.Generic;
using System;
using TwitchLib.Api.Helix.Models.Games;
using System.Text.RegularExpressions;
using MarkEKraus.DiscordWebhookService.Models;
using MarkEKraus.DiscordWebhookService.Interfaces;

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
        private static Dictionary<string, Game> _gameList = new Dictionary<string, Game>(StringComparer.InvariantCultureIgnoreCase);

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
            var message = _config.Value.TwitchChannels
                .Where(
                    a => a.Channel.Equals(args.Channel, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefault()
                .Message;

            var user = _twitchApi.Helix.Users
                .GetUsersAsync(logins: new List<string>(){ args.Channel })
                .GetAwaiter()
                .GetResult()
                .Users
                .FirstOrDefault();
            
            var webhookMessage = new WebhookMessage();
            webhookMessage.Content = ParseMessage(message, args);

            webhookMessage.Embeds = new List<IMessageEmbed>()
            {
                new MessageEmbed()
                {
                    Author = new MessageEmbedAuthor()
                    {
                        Name = args.Stream.UserName,
                        IconUrl = user.ProfileImageUrl,
                        Url = $"https://twitch.tv/{args.Channel}"
                    },
                    // Hex 9B59B6 "Dark Purple"
                    Color = 10181046,
                    Title = args.Stream.Title,
                    Url = $"https://twitch.tv/{args.Channel}",
                    Type = MessageEmbedType.rich,
                    Thumbnail = new MessageEmbedImage()
                    {
                        Url = user.ProfileImageUrl,
                        Height = 300,
                        Width = 300
                    },
                    Image = new MessageEmbedImage()
                    {
                        Url = args.Stream.ThumbnailUrl.Replace("{width}","320").Replace("{height}","180"),
                        Height = 180,
                        Width = 320
                    },
                    Fields = new List<IMessageEmbedField>()
                    {
                        new MessageEmbedField()
                        {
                            IsInline = true,
                            Name = "Game",
                            Value = GetGame(args.Stream.GameId).Name
                        },
                        new MessageEmbedField()
                        {
                            IsInline = true,
                            Name = "Viewers",
                            Value = args.Stream.ViewerCount.ToString()
                        }
                    },
                    Timestamp = DateTime.UtcNow
                }
            };

            _webhookService.SendMessageAsync(webhookMessage);
        }

        private string ParseMessage(string Message, OnStreamOnlineArgs args)
        {
            var replacements = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"{userName}", args.Stream.UserName},
                {"{channel}", args.Channel},
                {"{url}", $"https://twitch.tv/{args.Channel}"},
                {"{game}", GetGame(args.Stream.GameId).Name},
                {"{gameBoxArtUrl}", GetGame(args.Stream.GameId).BoxArtUrl},
                {"{streamLanguage}", args.Stream.Language},
                {"{streamType}", args.Stream.Type},
                {"{title}", args.Stream.Title},
                {"{viewerCount}", args.Stream.ViewerCount.ToString()},
                {"{startTime}", args.Stream.StartedAt.ToString()}
            };

            return Regex.Replace(
                                Message,
                                String.Join(
                                    "|",
                                    replacements
                                        .Keys
                                        .Select(k => k.ToString())
                                        .ToArray()),
                                m => replacements[m.Value],
                                RegexOptions.IgnoreCase);
        }

        private Game GetGame(string GameId)
        {
            Game response;
            if(_gameList.TryGetValue(GameId, out response))
            {
                return response;
            }

            response = _twitchApi.Helix.Games.GetGamesAsync(
                    new List<string>{GameId})
                        .GetAwaiter()
                        .GetResult()
                        .Games
                        .FirstOrDefault();
            _gameList.TryAdd(GameId, response);
            return response;
        }
    }
}