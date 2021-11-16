using MixMods.MixPackageManager.Utils;
using System;
using System.IO;

namespace MixMods.MixPackageManager
{
    public class Settings
    {
        private static string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static string mpmFolder = ".mpm";
        private static string dir = Path.Combine(userFolder, mpmFolder);
        private static string path = Path.Combine(dir, "settings.ini");

        private static void CheckFile()
        {
            var dir = Path.Combine(userFolder, mpmFolder);
            var path = Path.Combine(dir, "settings.ini");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
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
