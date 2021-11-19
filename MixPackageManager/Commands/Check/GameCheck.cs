using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace MixMods.MixPackageManager
{
    public class GameCheck
    {
        [Command("check-game")]
        public static void Check(Arguments arguments)
        {
            CheckInstallPath();
            CheckFiles();
        }
        private static void CheckInstallPath()
        {
            if (Program.fullPath.Contains(@"C:\Program Files"))
            {
                Program.Error(@"It is not recommended to install the game in the C:\Program Files folder!");
            }
        }
        private static void CheckFiles()
        {
            var client = new WebClient();
            var sourceFilesText = client.DownloadString("https://raw.githubusercontent.com/chrystianfarias/mpm/main/internal_files/gtasa_files.txt");
            var sourceFiles = sourceFilesText.Split(Environment.NewLine.ToCharArray());
            
            foreach(var file in sourceFiles)
            {
                var name = file.Split('#')[0];
                var size = long.Parse(file.Split('#')[1]);
                var path = Program.fullPath + name;
                if (File.Exists(path))
                {
                    long length = new FileInfo(path).Length;
                    if (length != size)
                        Program.Error($"File {name} is modified!");
                }
                else
                {
                    Program.Error($"File {name} is not in game folder!");
                }
            }
        }
    }
}
