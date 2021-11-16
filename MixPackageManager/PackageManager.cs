using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
                InstallPackage(arguments[1]);
            }
        }
        public void InstallPackage(string package)
        {
            Console.WriteLine("{download}");
            var i = 0;
            while (i <= 100)
            {
                i++;
                Console.WriteLine(i);
                Thread.Sleep(50);
            }
            Console.WriteLine("{install}");
            i = 0;
            while (i <= 100)
            {
                i++;
                Console.WriteLine(i);
                Thread.Sleep(20);
            }
        }
        public void InstallPackageJson(Arguments arguments)
        {
            if (File.Exists("mods.json"))
            {
                Console.WriteLine(File.ReadAllText("mods.json"));
            }
            else
            {
                Program.Error("mods.json not found! Run 'mpm init' first");
            }
        }
        [Command("init")]
        public void InitPackageJson(Arguments arguments)
        {
            Console.WriteLine("Init!!!!!");
            Settings.Save("teste", "1234");
        }
    }
}
