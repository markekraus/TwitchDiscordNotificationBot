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
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.IO;

namespace MarkEKraus.TwitchDiscordNotificationBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        var configuration = new ConfigurationBuilder()
                            .SetBasePath(GetBasePath())
                            .AddJsonFile("appsettings.json", false)
                            .AddEnvironmentVariables($"{nameof(MarkEKraus.TwitchDiscordNotificationBot).ToUpper()}_")
                            .AddUserSecrets<AppSettings>()
                            .Build();
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
                                var logFolder = Path.Join(GetBasePath(),"Logs");
                                logging.AddFile($"{logFolder}/{nameof(MarkEKraus.TwitchDiscordNotificationBot)}-{{Date}}.txt");
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
                        //services.AddTransient<ConsoleApplication>();
                        services.AddHostedService<ConsoleApplication>();
                    }
                );
        }

        private static string GetBasePath()
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }
    }
}
