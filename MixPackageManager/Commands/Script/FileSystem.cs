using System;
using System.IO;
using IniParser;
using IniParser.Model;

namespace MixMods.MixPackageManager.Commands.Script
{ 
    public class FileSystem
    {
        public INI openIni(string path)
        {
            Console.WriteLine("Open Ini: " + path);
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(path);
            return new INI(data, path);
        }
        public string readFile(string path)
        {
            return File.ReadAllText(path);
        }
        public void writeFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }
        public bool fileExists(string path)
        {
            return File.Exists(path);
        }
        public void deleteFile(string path)
        {
            File.Delete(path);
        }
        public void moveFile(string origin, string target)
        {
            File.Move(origin, target);
        }
        public void createDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        public void deleteDirectory(string path)
        {
            Directory.Delete(path);
        }
        public bool directoryExists(string path)
        {
            return Directory.Exists(path);
        }
        public string[] getFiles(string path, string filter)
        {
            return Directory.GetFiles(path, filter);
        }
        public string[] getDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}
