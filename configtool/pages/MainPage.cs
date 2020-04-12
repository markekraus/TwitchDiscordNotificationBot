using System;
using EasyConsole;

namespace configtool.pages
{
    public class MainPage : MenuPage
    {
        public MainPage(Program program)
            : base("Main Menu", program,
            new Option("Register/Unregister/Start/Stop/Disable Bot Service", () => program.NavigateTo<ServiceConfigPage>()),
            new Option("Configure Bot Settings", () => program.NavigateTo<BotConfigPage>()),
            new Option("Exit Configtool", () => Environment.Exit(0))
            )
        {
        }
    }
}