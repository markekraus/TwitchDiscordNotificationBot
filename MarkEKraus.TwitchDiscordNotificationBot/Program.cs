using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Interfaces;
using MarkEKraus.DiscordWebhookService;
using MarkEKraus.DiscordWebhookService.Interfaces;

namespace MarkEKraus.TwitchDiscordNotificationBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            await serviceProvider.GetService<ConsoleApplication>().Run();
        }

        private static IServiceCollection ConfigureServices()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables($"{nameof(MarkEKraus.TwitchDiscordNotificationBot).ToUpper()}_")
                .AddUserSecrets<AppSettings>()
                .Build();
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<AppSettings>(configuration);
            services.AddLogging(logging => 
            {
                logging
                    .ClearProviders()
                    .SetMinimumLevel(LogLevel.Information);

                if(configuration.GetValue<bool>("EnableConsoleLogging"))
                {
                    logging.AddConsole( c =>
                    {
                        c.TimestampFormat = "[HH:mm:ss] ";
                    });
                }

                if(configuration.GetValue<bool>("EnableFileLogging"))
                {
                    logging.AddFile($"Logs/{nameof(MarkEKraus.TwitchDiscordNotificationBot)}-{{Date}}.txt");
                }

            });
            services.AddSingleton<IApiSettings>( sp => new ApiSettings(){ 
                ClientId = configuration.GetValue<string>("TwitchApiClientId"),
                Secret = configuration.GetValue<string>("TwitchApiClientSecret")
            });

            services.Configure<WebhookOptions>(option => { 
                option.WebhookUri = configuration.GetValue<Uri>("DiscordWebHookUri");});

            services.AddHttpClient<IWebhookService, WebhookService>();

            services.AddSingleton<ITwitchAPI, TwitchAPI>();
            services.AddTransient<ConsoleApplication>();
            return services;
        }
    }
}
