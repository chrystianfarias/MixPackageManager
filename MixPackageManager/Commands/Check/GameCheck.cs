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
            try
            {
                CheckInstallPath();
                CheckExecutable();
                CheckFiles();
            }
            catch(Exception ex)
            {
                Program.Error(ex.Message);
            }
        }
        private static void CheckInstallPath()
        {
            if (Program.fullPath.Contains(@"C:\Program Files"))
            {
                Console.WriteLine(Program.isGui ? $"path#notrecommended" : @"It is not recommended to install the game in the C:\Program Files folder!");
            }
        }
        private static void CheckExecutable()
        {
            var info = new FileInfo(Path.Combine(Program.fullPath, "gta_sa.exe"));
            if (info.Length != 14383616)
            {
                Console.WriteLine(Program.isGui ? $"exe#notrecommended" : @"Use hoodlum executable");
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
                        Console.WriteLine(Program.isGui ? $"modified#{name}" : $"File {name} is modified!");
                }
                else
                {
                    Console.WriteLine(Program.isGui ? $"missing#{name}" : $"File {name} is not in game folder!");
                }
            }
        }
    }
}
