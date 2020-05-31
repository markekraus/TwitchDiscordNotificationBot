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
using TwitchLib.Api.Core.Exceptions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Net.Sockets;
using System.Net.Http;

namespace MarkEKraus.TwitchDiscordNotificationBot
{
    internal class ConsoleApplication : BackgroundService
    {
        private ILogger<ConsoleApplication> _logger;
        private IOptions<AppSettings> _config;
        private ITwitchAPI _twitchApi;
        private IApiSettings _apiSettings;
        private LiveStreamMonitorService _twitchMonitor;
        private IWebhookService _webhookService;
        private static Dictionary<string, Game> _gameList = new Dictionary<string, Game>(StringComparer.InvariantCultureIgnoreCase);
        private DateTime _startNotificationTime;
        private const int _delayNotificationsSeconds = 20;
        private CancellationToken _token;

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
            _token = new CancellationToken();
        }

        protected override async Task ExecuteAsync(CancellationToken cancelToken)
        {
            _token = cancelToken;
            await Run();
        }

        public async Task Run()
        {
            _logger.LogInformation($"{nameof(MarkEKraus.TwitchDiscordNotificationBot)} Start");
            
            _twitchMonitor.SetChannelsByName(
                _config.Value.TwitchChannels
                .Select(channel => channel.Channel)
                .ToList());
            
            _twitchMonitor.OnStreamOnline += Monitor_OnStreamOnline;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(TwitchApiExceptionHandler);

            // TwitchLib monintors treat already streaming streams as an OnStreamOnline.
            // This workaround uses a delay to ignore OnStreamOnline for a few seconds
            // after the monitor's first interval.
            _startNotificationTime = DateTime.Now.AddSeconds(_twitchMonitor.IntervalInSeconds + _delayNotificationsSeconds);

            _twitchMonitor.Start();


            await Task.Delay(-1,_token);
        }

        private void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs args)
        {
            if(_config.Value.SkipActiveStreamsOnStartup && DateTime.Now < _startNotificationTime)
            {
                _logger.LogInformation($"Skipping Active Stream {args.Channel}");
                return;
            }

            _logger.LogInformation($"channel: {args.Channel}");
            var message = _config.Value.TwitchChannels
                .Where(
                    a => a.Channel.Equals(args.Channel, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault()
                .Message;

            var user = _twitchApi.Helix.Users
                .GetUsersAsync(logins: new List<string>(){ args.Channel })
                .GetAwaiter()
                .GetResult()
                .Users
                .FirstOrDefault();
            
            var webhookMessage = new WebhookMessage();

            try
            {
                webhookMessage.Content = ParseMessage(message, args);
                _logger.LogInformation($"Message: {webhookMessage.Content}");
            }
            catch (System.Exception e)
            {
                _logger.LogCritical(e,$"Unable to parse message: {e.Message}");
                return;
            }

            string tags = string.Empty;
            List<string> tagsList = new List<string>();
            try
            {
                var tagsResult = _twitchApi.Helix.Streams
                    .GetStreamTagsAsync(args.Stream.UserId)
                    .GetAwaiter()
                    .GetResult()
                    .Data;
                foreach (var tag in tagsResult)
                {
                    string localizedTag;
                    if(tag.LocalizationNames.TryGetValue("en-us",out localizedTag))
                    {
                        tagsList.Add(localizedTag);
                    }
                }
                tags = string.Join(", ",tagsList);
            }
            catch (System.Exception e)
            {
                _logger.LogCritical(e, $"Unable to retrieve tags: {e.Message}");
            }

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
                        Url = args.Stream.ThumbnailUrl.Replace("{width}","320").Replace("{height}","180") + "?" + DateTime.UtcNow.ToFileTimeUtc().ToString(),
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
                        },
                        new MessageEmbedField()
                        {
                            IsInline = true,
                            Name = "Tags",
                            Value = tags
                        }
                    },
                    Timestamp = DateTime.UtcNow
                }
            };

            _webhookService.SendMessageAsync(webhookMessage).GetAwaiter().GetResult();
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

        private void TwitchApiExceptionHandler (object sender, UnhandledExceptionEventArgs args)
        {
            if(args.ExceptionObject is InternalServerErrorException e)
            {
                _logger.LogError($"Caught {nameof(InternalServerErrorException)} exception: {e.Message}{Environment.NewLine}{e.StackTrace}");
                _twitchMonitor.Stop();
                _twitchMonitor.Start();
            }
            else if (args.ExceptionObject is SocketException e2)
            {
                _logger.LogError($"Caught {nameof(SocketException)} exception: {e2.Message}{Environment.NewLine}{e2.StackTrace}");
                _twitchMonitor.Stop();
                _twitchMonitor.Start();
            }
            else if (args.ExceptionObject is HttpRequestException e3)
            {
                _logger.LogError($"Caught {nameof(HttpRequestException)} exception: {e3.Message}{Environment.NewLine}{e3.StackTrace}");
                _twitchMonitor.Stop();
                _twitchMonitor.Start();
            }
            else
            {
                var ex = (Exception)args.ExceptionObject;
                _logger.LogCritical($"Unhandled exception {args.ExceptionObject.GetType().FullName}. {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
}