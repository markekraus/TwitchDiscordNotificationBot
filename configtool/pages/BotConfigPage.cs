using System;
using System.IO;
using EasyConsole;
using Newtonsoft.Json;

namespace configtool.pages
{
    public class BotConfigPage : MenuPage
    {
        private static string _appSettingsFile;
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
                new Option("Exit Configtool", () => Environment.Exit(0)))
        {
            _appSettingsFile = Path.Join(ConsoleProgram.GetBasePath(),"appSettings.json");
        }

        private static void SetDiscordWebHookUri()
        {
            Console.WriteLine("Enter Twitch App Client Secret: ");
            var webhook = Console.ReadLine();
            UpdateFile("TwitchApiClientSecret",webhook);
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

        private static void UpdateFile(string setting, string value)
        {
            Console.WriteLine($"Updating {setting}...");
            Console.WriteLine($"Getting current settings from {_appSettingsFile}");
            var fileContents = File.ReadAllText(_appSettingsFile);
            dynamic jsonObj = JsonConvert.DeserializeObject(fileContents);
            Console.WriteLine($"Previous setting: {jsonObj[setting]}");
            Console.WriteLine($"New setting: {value}");
            jsonObj[setting] = value;
            var outContents = JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_appSettingsFile, outContents);
            Console.WriteLine($"Updated {setting}!");
        }
    }
}