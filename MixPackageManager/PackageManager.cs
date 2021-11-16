using System;
using System.IO;

namespace MixMods.MixPackageManager
{
    public class PackageManager
    {
        [Command("install")]
        public void Install(Arguments arguments)
        {
            if (arguments.Length < 2)
            {
                InstallPackageJson(arguments);
                return;
            }
            else
            {
                Console.WriteLine(arguments[1]);
            }
        }
        public void InstallPackageJson(Arguments arguments)
        {
            if (File.Exists("packages.json"))
            {
                Console.WriteLine(File.ReadAllText("packages.json"));
            }
            else
            {
                Program.Error("packages.json not found! Run 'mpm init' first");
            }
        }
        [Command("init")]
        public void InitPackageJson(Arguments arguments)
        {
            Console.WriteLine("Init!!!!!");
        }
    }
}
