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
            Console.WriteLine("mpm install\t\tinstall/update all mods in package");
            Console.WriteLine("mpm install <mod>\tinstall a specific mod");
            Console.WriteLine("mpm help\t\thelp commands");
            Console.WriteLine("mpm init\t\tinit Mix Package Manager");
        }
    }
}
