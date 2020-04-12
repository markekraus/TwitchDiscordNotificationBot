using System;
using System.IO;
using EasyConsole;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace configtool.pages
{
    public class ChannelConfigPage : MenuPage
    {
        private const string _twitchChannels = "TwitchChannels";
        private const string _channel = "Channel";
        private const string _message = "Message";
        public ChannelConfigPage(Program program)
            : base("Twitch Channels Configuration", program,
            new Option("Show channels", () =>
            {
                ShowChannels();
                ConsoleProgram.PressToContinue();
                program.NavigateTo<ChannelConfigPage>();
            }),
            new Option("Add channel", () =>
            {
                AddChannel();
                ConsoleProgram.PressToContinue();
                program.NavigateTo<ChannelConfigPage>();
            }),
            new Option("Remove channel", () =>
            {
                RemoveChannel();
                ConsoleProgram.PressToContinue();
                program.NavigateTo<ChannelConfigPage>();
            }),
            new Option("Exit Configtool", () => Environment.Exit(0)))
        {
        }

        private static void RemoveChannel()
        {
            var config = GetConfig();
            var channels = (JArray)config[_twitchChannels];
            var menu = new Menu();
            foreach (JObject item in channels)
            {
                menu.Add((string)item[_channel],() =>
                {
                    Console.WriteLine($"Removing {item[_channel]}...");
                    channels.Remove(item);
                });
            }
            menu.Display();
            UpdateConfig(config);
        }

        private static void AddChannel()
        {
            var channel = Input.ReadString("Enter twitch channel name:").ToLower();
            Console.WriteLine(" ");
            Console.WriteLine("To learn more about message formatting:");
            Console.WriteLine("https://github.com/markekraus/TwitchDiscordNotificationBot#message-formatting");
            Console.WriteLine(" ");
            var message = Input.ReadString("Enter the discord message:").ToLower();
            var config = GetConfig();
            var channels = (JArray)config[_twitchChannels];
            var newChannel = new JObject();
            newChannel[_channel] = channel;
            newChannel[_message] = message;
            channels.Add(newChannel);
            UpdateConfig(config);
        }

        private static void ShowChannels()
        {
            var config = GetConfig();
            foreach (var item in config[_twitchChannels])
            {
                Console.WriteLine($"Channel: {item[_channel]}");
                Console.WriteLine($"Message: {item[_message]}");
                Console.WriteLine(" ");
            }
        }

        private static dynamic GetConfig()
        {
            var appSettingsFile = Path.Join(ConsoleProgram.GetBasePath(),"appSettings.json");
            Console.WriteLine($"Getting current settings from {appSettingsFile}");
            var fileContents = File.ReadAllText(appSettingsFile);
            var jsonObj = JObject.Parse(fileContents);
            return jsonObj;
        }

        private static void UpdateConfig(JObject config)
        {
            var appSettingsFile = Path.Join(ConsoleProgram.GetBasePath(),"appSettings.json");
            File.WriteAllText(appSettingsFile, config.ToString());
            Console.WriteLine("Configuration updated!");
        }
    }
}