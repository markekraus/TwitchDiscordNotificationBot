using System;
using System.Diagnostics;
using System.IO;
using EasyConsole;

namespace configtool.pages
{
    public class ServiceConfigPage : MenuPage
    {
        private const string _serviceName = "TwitchDiscordNotificationBot";
        private const string _exeName = "TwitchDiscordNotificationBot.exe";
        private const string _serviceDisplayName = "Twitch Discord Notification Bot";
        private const string _serviceDescription = "A bot which sends Discord notifications when a streamer goes live on twitch. https://github.com/markekraus/TwitchDiscordNotificationBot";
        public ServiceConfigPage(Program program)
            : base("Bot Service Configuration", program,
                new Option("Get Service Status", () =>
                {
                    QueryService();
                    program.NavigateTo<ServiceConfigPage>();
                }),
                new Option("Register Service", () =>
                {
                    RegisterService();
                    program.NavigateTo<ServiceConfigPage>();
                }),
                new Option("Unregister Service", () =>
                {
                    UnregisterService();
                    program.NavigateTo<ServiceConfigPage>();
                }),
                new Option("Start Service", () =>
                {
                    StartService();
                    program.NavigateTo<ServiceConfigPage>();
                }),
                new Option("Stop Service", () =>
                {
                    StopService();
                    program.NavigateTo<ServiceConfigPage>();
                }),
                new Option("Disable Service", () =>
                {
                    DisableService();
                    program.NavigateTo<ServiceConfigPage>();
                }),
                new Option("Enable Service", () =>
                {
                    EnableService();
                    program.NavigateTo<ServiceConfigPage>();
                }),
                new Option("Exit Configtool", () => Environment.Exit(0)))
        {}

        private static void QueryService()
        {
            Console.WriteLine("Querying service...");
            ExecSc($"query {_serviceName}");
            Console.WriteLine("Service queried!");
            ConsoleProgram.PressToContinue();
        }

        private static void EnableService()
        {
            Console.WriteLine("Enabling service...");
            ExecSc($"config {_serviceName} start=auto");
            Console.WriteLine("Service enabled!");
            ConsoleProgram.PressToContinue();
        }

        private static void DisableService()
        {
            Console.WriteLine("Disabling service...");
            ExecSc($"config {_serviceName} start=disabled");
            Console.WriteLine("Service disabled!");
            ConsoleProgram.PressToContinue();
        }

        private static void StopService()
        {
            Console.WriteLine("Stopping service...");
            ExecSc($"stop {_serviceName}");
            Console.WriteLine("Service Stopped!");
            ConsoleProgram.PressToContinue();
        }

        private static void StartService()
        {
            Console.WriteLine("Starting service...");
            ExecSc($"start {_serviceName}");
            Console.WriteLine("Service Started!");
            ConsoleProgram.PressToContinue();
        }

        private static void UnregisterService()
        {
            Console.WriteLine("Unregistering service...");
            ExecSc($"delete {_serviceName}");
            Console.WriteLine("Service Unregistered!");
            ConsoleProgram.PressToContinue();
        }

        private static void RegisterService()
        {
            System.Console.WriteLine("Registering service...");
            var processPath = ConsoleProgram.GetBasePath();
            var exePath = Path.Join(processPath,_exeName);
            var args = $"create {_serviceName} start=auto binpath=\"{exePath}\" displayname=\"{_serviceDisplayName}\"";
            ExecSc(args);
            args = $"description {_serviceName} \"{_serviceDescription}\"";
            ExecSc(args);
            args = $"failure {_serviceName} actions= restart/60000ms/restart/60000/restart/60000ms// reset= 3600000";
            ExecSc(args);
            System.Console.WriteLine("Service Registered!");
            ConsoleProgram.PressToContinue();
        }

        private static void ExecSc(string args)
        {
            System.Console.WriteLine($"sc {args}");
            var proc = Process.Start("sc", args);
            while (!proc.HasExited)
            {}
        }

        
    }
}