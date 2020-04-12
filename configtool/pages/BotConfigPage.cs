using System;
using System.IO;
using EasyConsole;
using Newtonsoft.Json;

namespace configtool.pages
{
    public class BotConfigPage : MenuPage
    {
        public BotConfigPage(Program program)
            : base ("Bot Settings Configuration", program,
                new Option("Set TwitchApiClientId", () => 
                {
                    SetTwitchApiClientId();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Set TwitchApiClientSecret", () => 
                {
                    SetTwitchApiClientSecret();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Set DiscordWebHookUri", () => 
                {
                    SetDiscordWebHookUri();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Set TwitchApiCheckIntervalSeconds", () => 
                {
                    SetTwitchApiCheckIntervalSeconds();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Enable console logging", () => 
                {
                    UpdateFile("EnableConsoleLogging",true);
                    ConsoleProgram.PressToContinue();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Disable console logging", () => 
                {
                    UpdateFile("EnableConsoleLogging",false);
                    ConsoleProgram.PressToContinue();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Enable file logging", () => 
                {
                    UpdateFile("EnableFileLogging",true);
                    ConsoleProgram.PressToContinue();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Disable file logging", () => 
                {
                    UpdateFile("EnableFileLogging",false);
                    ConsoleProgram.PressToContinue();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Skip active streams on startup", () => 
                {
                    UpdateFile("SkipActiveStreamsOnStartup",true);
                    ConsoleProgram.PressToContinue();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Notify active streams on startup", () => 
                {
                    UpdateFile("SkipActiveStreamsOnStartup",false);
                    ConsoleProgram.PressToContinue();
                    program.NavigateTo<BotConfigPage>();
                }),
                new Option("Manage Channels", () => program.NavigateTo<ChannelConfigPage>()),
                new Option("Exit Configtool", () => Environment.Exit(0)))
        {
        }

        private static void SetTwitchApiCheckIntervalSeconds()
        {
            bool isNumber = false;
            int interval;
            do
            {
                Console.WriteLine("Enter Twitch API Check Interval Seconds: ");
                var webhook = Console.ReadLine();
                isNumber = int.TryParse(webhook,out interval);

                if(!isNumber)
                {
                    Console.WriteLine("Response must be a number.");
                }

            } while (!isNumber);

            UpdateFile("TwitchApiCheckIntervalSeconds",interval);
            ConsoleProgram.PressToContinue();
        }

        private static void SetDiscordWebHookUri()
        {
            Console.WriteLine("Enter Discord Webhook Uri: ");
            var webhook = Console.ReadLine();
            UpdateFile("DiscordWebHookUri",webhook);
            ConsoleProgram.PressToContinue();
        }

        private static void SetTwitchApiClientSecret()
        {
            Console.WriteLine("Enter Twitch App Client Secret: ");
            var secret = Console.ReadLine();
            UpdateFile("TwitchApiClientSecret",secret);
            ConsoleProgram.PressToContinue();
        }

        private static void SetTwitchApiClientId()
        {
            Console.WriteLine("Enter Twitch App Client ID: ");
            var clientId = Console.ReadLine();
            UpdateFile("TwitchApiClientId",clientId);
            ConsoleProgram.PressToContinue();
        }

        internal static void UpdateFile(string setting, object value)
        {
            var appSettingsFile = Path.Join(ConsoleProgram.GetBasePath(),"appSettings.json");
            Console.WriteLine($"Updating {setting}...");
            Console.WriteLine($"Getting current settings from {appSettingsFile}");
            var fileContents = File.ReadAllText(appSettingsFile);
            dynamic jsonObj = JsonConvert.DeserializeObject(fileContents);
            Console.WriteLine($"Previous setting: {jsonObj[setting]}");
            Console.WriteLine($"New setting: {value}");
            if(value is string stringValue)
            {
                jsonObj[setting] = stringValue;
            }
            if(value is int intValue)
            {
                jsonObj[setting] = intValue;
            }
            if(value is bool boolValue)
            {
                jsonObj[setting] = boolValue;
            }
            var outContents = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(appSettingsFile, outContents);
            Console.WriteLine($"Updated {setting}!");
        }


    }
}