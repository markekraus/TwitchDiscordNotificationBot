
using configtool.pages;
using EasyConsole;

namespace configtool
{
    public class BotConfigProgram : Program
    {
        public BotConfigProgram()
            : base("Bot Configuration Tool", breadcrumbHeader: true)
        {
            AddPage(new MainPage(this));
            AddPage(new BotConfigPage(this));
            AddPage(new ServiceConfigPage(this));
            AddPage(new ChannelConfigPage(this));

            SetPage<MainPage>();
        }
    }
}