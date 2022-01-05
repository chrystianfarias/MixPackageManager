using System;
using System.Windows.Forms;

namespace MixMods.MixPackageManager
{
    public class Version
    {
        [Command("v")]
        public static void Print(Arguments arguments)
        {
            Console.WriteLine(Application.ProductVersion);
        }
    }
}
