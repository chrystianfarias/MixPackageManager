using Jint;
using MixMods.MixPackageManager.Commands.Script;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            try
            {
                var engine = new Engine()
                    .SetValue("log", new Action<object>(Console.WriteLine))
                    .SetValue("message", new Action<string, string, string>(OpenMessage))
                    .SetValue("createDialog", new Func<Dialog>(Dialog.CreateDialog))
                    .SetValue("openIni", new Func<string, INI>(INI.OpenIni));

                engine.Execute(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static void OpenMessage(string title, string description, string type = "")
        {
            MessageBoxIcon icon;
            switch(type)
            {
                case "info":
                    icon = MessageBoxIcon.Information;
                    break;
                case "error":
                    icon = MessageBoxIcon.Error;
                    break;
                case "warning":
                    icon = MessageBoxIcon.Warning;
                    break;
                default:
                    icon = MessageBoxIcon.None;
                    break;
            }
            MessageBox.Show(title, description, MessageBoxButtons.OK, icon);
        }
    }
}
