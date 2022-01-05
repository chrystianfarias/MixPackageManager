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
                var fs = new FileSystem();
                var engine = new Engine()
                    .SetValue("print", new Action<object>(Console.WriteLine))
                    .SetValue("message", new Action<string, string, string>(OpenMessage))
                    .SetValue("IO", fs)
                    .SetValue("Dialog", typeof(Dialog));

                engine.Execute(File.ReadAllText(path));
            }
            catch (Jint.Parser.ParserException ex)
            {
                OpenMessage("Syntax error: " + ex.Message, "Script Error", "error");
            }
            catch (Exception ex)
            {
                OpenMessage(ex.Message, "Error", "error");
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
