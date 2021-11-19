using MixMods.MixPackageManager.Utils;
using System;
using System.IO;

namespace MixMods.MixPackageManager
{
    public class Settings
    {
        private static string dir = ".mpm";
        private static string path = Path.Combine(dir, "settings.ini");

        private static void CheckFile()
        {
            var path = Path.Combine(dir, "settings.ini");
            if (!Directory.Exists(dir))
            {
                var d = Directory.CreateDirectory(dir);
                d.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            if (!File.Exists(path))
                File.WriteAllText(path, "[Settings]");
        }
        
        public static string Load(string key, string section = "Settings")
        {
            CheckFile();
            var ini = new IniFile(path);
            return ini.Read(key, section);
        }

        public static void Save(string key, string value, string section="Settings")
        {
            CheckFile();
            var ini = new IniFile(path);
            ini.Write(key, value, section);
        }
    }
}
