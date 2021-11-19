using Jint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMods.MixPackageManager
{
    public class ScriptManager
    {
        [Command("run")]
        public static void ExecuteScriptCommand(Arguments arguments)
        {
            if (arguments.Length < 2)
            {
                Program.Error("Usage: mpm run <file.js>");
                return;
            }
            else
            {
                ExecuteScript(arguments[1]);
            }
        }
        public static void ExecuteScript(string path)
        {
            var engine = new Engine()
                .SetValue("log", new Action<object>(Console.WriteLine));

            engine.Execute(File.ReadAllText(path));
        }
    }
}
