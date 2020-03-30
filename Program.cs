using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Interfaces;

namespace TwitchDiscordNotificationBot
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
                .AddEnvironmentVariables($"{nameof(TwitchDiscordNotificationBot).ToUpper()}_")
                .AddUserSecrets<AppSettings>()
                .Build();
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<AppSettings>(configuration);
            services.AddLogging(logging => 
            {
                logging
                    .ClearProviders()
                    .SetMinimumLevel(LogLevel.Information)
                    .AddConsole( c =>
                    {
                        c.TimestampFormat = "[HH:mm:ss] ";
                    });
            });
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IApiSettings>( sp => new ApiSettings(){ 
                ClientId = configuration.GetValue<string>("TwitchApiClientId"),
                Secret = configuration.GetValue<string>("TwitchApiClientSecret")
            });
            services.AddSingleton<TwitchAPI>();
            services.AddTransient<ConsoleApplication>();
            return services;
        }
    }
}
