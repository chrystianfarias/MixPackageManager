using System;

namespace MixMods.MixPackageManager
{
    public class Help
    {
        [Command("help")]
        public static void Print(Arguments arguments)
        {
            Console.WriteLine("mpm <command>");
            Console.WriteLine("");
            Console.WriteLine("Usage");
            Console.WriteLine("");
            Console.WriteLine("mpm install");
            Console.WriteLine("mpm help");
        }
    }
}
