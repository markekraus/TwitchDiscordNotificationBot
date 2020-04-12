using System;
using System.Diagnostics;
using System.IO;

namespace configtool
{
    class ConsoleProgram
    {
        static void Main(string[] args)
        {
            new BotConfigProgram().Run();
        }

        internal static string GetBasePath()
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }

        internal static void PressToContinue()
        {
            System.Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
