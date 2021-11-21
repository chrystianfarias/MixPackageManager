using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using MixMods.MixPackageManager.Models;
using Newtonsoft.Json;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace MixMods.MixPackageManager
{
    public class PackageManager
    {
        [Command("install")]
        public void InstallPackage(Arguments arguments)
        {
            if (arguments.Length < 2)
            {
                InstallPackageJson(arguments);
                return;
            }
            else
            {
                var arg = arguments[1];

                if (!arg.Contains("@"))
                    arg = $"{arg}@latest";

                InstallPackage(arg);
            }
        }
        [Command("check-package")]
        public void CheckInstalledMod(Arguments arguments)
        {
            if (arguments.Length >= 2)
            {
                var arg = arguments[1];

                if (!arg.Contains("@"))
                    arg = $"{arg}@latest";

                CheckInstalledMod(arg);
            }
        }
        public bool CheckInstalledMod(string package)
        {
            var packageInfo = package.Split('@'); //example: mods/modA@latest.json
            var packageName = packageInfo[0];  //example: modA
            var packageVersion = packageInfo[1].Split('.')[0]; //example: latest
            var mods = GetInstalledPackages();
            var existentPackage = mods.FirstOrDefault(mod =>
            {
                var modInfo = mod.Split('@');
                var modName = modInfo[0];
                var modVersion = modInfo[1].Split('.')[0];

                return modName == packageName && modVersion == packageVersion;
            });
            if (existentPackage != null)
                Console.WriteLine(Program.isGui ? $"package#true" : $"Package {packageName} with version {packageVersion} is installed");
            else
                Console.WriteLine(Program.isGui ? $"package#false" : $"Package {package} is not installed");
            return existentPackage != null;
        }
        public void InstallPackage(string package)
        {
            if (!File.Exists(Path.Combine(Program.fullPath, "mods.json")))
            { 
                Program.Error("mods.json not found! Run 'mpm init' first");
                return;
            }
            var mods = GetInstalledPackages();
            var packageInfo = package.Split('@'); //example: mods/modA@latest.json
            var packageName = packageInfo[0];  //example: modA
            var packageVersion = packageInfo[1].Split('.')[0]; //example: latest

            mods.FirstOrDefault(m =>
            {
                var modInfo = m.Split('@');
                var modName = modInfo[0];
                var modVersion = modInfo[1].Split('.')[0];

                return modName == packageName && modVersion == packageVersion;
            }); //Is installed

            var jsonByte = Download(new Uri($"{Program.API}mods/{package}.json"));
            var jsonString = System.Text.Encoding.Default.GetString(jsonByte);
            var mod = JsonConvert.DeserializeObject<Mod>(jsonString);

            //Install dependencies
            foreach (var dependency in mod.DependencyMods)
                InstallPackage(dependency);
                    
            var modFile = Download(new Uri(mod.Url));
            var modStream = new MemoryStream(modFile);
            var extractModLoaderFolder = Path.Combine(Program.fullPath, mod.ToDirectory.Replace("{package}", packageName));
            try
            {
                //If file directory not exists, create
                if (!Directory.Exists(extractModLoaderFolder))
                    Directory.CreateDirectory(extractModLoaderFolder);

                //Create temp folder
                var tempFolder = Path.Combine(extractModLoaderFolder, "temp");
                Directory.CreateDirectory(tempFolder);
                            
                using (ArchiveFile archiveFile = new ArchiveFile(modStream))
                {
                    Console.WriteLine(Program.isGui ? $"extract#" : $"Extracting {packageName} package");
                    //Extract to temp folder.7z
                    archiveFile.Extract(tempFolder, true);
                }
                var extractDir = Path.Combine(tempFolder, mod.ExtractDir);
                var directiories = Directory.GetDirectories(extractDir);
                var files = Directory.GetFiles(extractDir);
                //Move Directories
                foreach (var dir in directiories)
                {
                    CopyAll(new DirectoryInfo(Path.Combine(Program.fullPath, dir)), 
                            new DirectoryInfo(Path.Combine(Program.fullPath, extractModLoaderFolder, Path.GetFileName(dir))));
                }
                //Move Files
                foreach (var file in files)
                {
                    var path = Path.Combine(extractModLoaderFolder, Path.GetFileName(file));
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                        Console.WriteLine($"File {Path.GetFileName(path)} replaced.");
                    }
                    File.Move(file, path);
                }

                //Delete temp folder
                Directory.Delete(tempFolder, true);
                            
                //Save .mod file
                File.WriteAllText(Path.Combine(extractModLoaderFolder, $"{packageName}.mod"), jsonString);
                            
            }
            catch (IOException ex)
            {
                Program.Error("Execute with administrator!");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(2);
            }

            Console.WriteLine(Program.isGui ? $"install#" : $"Installing {packageName} package");
            //TODO: installation

            mods.Add(package);
            SetInstalledPackages(mods);

            Console.WriteLine(Program.isGui ? $"complete" : $"Package {packageName} Installed");
        }
        private static byte[] Download(Uri uri)
        {
            byte[] downloaded = null;
            try
            {
                var client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                client.DownloadDataAsync(uri);
                client.DownloadDataCompleted += (s, e) =>
                {
                    downloaded = e.Result;
                };
                while (downloaded == null)
                {
                    Thread.Sleep(200);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var resp = (HttpWebResponse)ex.Response;
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        Program.Error("Package not found!");
                        return null;
                    }
                    if (resp.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        Program.Error("Server Error!");
                        return null;
                    }
                    Program.Error(resp.StatusDescription);
                    return null;
                }
                Program.Error(ex.Message);
            }
            return downloaded;
        }
        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        private static void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            if (Program.isGui)
                Console.WriteLine("download#{0}#",
                    e.ProgressPercentage);
            else
                Console.WriteLine("{0}    downloaded {1} of {2} bytes. {3} % complete...",
                        (string)e.UserState,
                        e.BytesReceived,
                        e.TotalBytesToReceive,
                        e.ProgressPercentage);
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
        public void SetInstalledPackages(List<string> mods)
        {
            var json = JsonConvert.SerializeObject(mods);
            File.WriteAllText("mods.json", json);
        }
        public List<string> GetInstalledPackages()
        {
            if (File.Exists(Path.Combine(Program.fullPath, "mods.json")))
            {
                var modsText = File.ReadAllText(Path.Combine(Program.fullPath, "mods.json"));
                return JsonConvert.DeserializeObject<List<string>>(modsText);
            }
            else
            {
                Program.Error("mods.json not found! Run 'mpm init' first");
            }
            return null;
        }

        [Command("init")]
        public void Init(Arguments arguments)
        {
            if (File.Exists(Path.Combine(Program.fullPath, "gta_sa.exe")))
            {
                File.WriteAllText(Path.Combine(Program.fullPath, "mods.json"), "[]");
            }
            else
            {
                Program.Error("run command in GTA SA root folder!");
            }
        }

        [Command("get-mods")]
        public void GetMods(Arguments arguments)
        {
            var isModloader = arguments.Contains("-modloader");
            var isMPM = arguments.Contains("-modmpm");
            if (isModloader == false && isMPM == false)
            {
                Program.Error("Select -modloader or -modmpm!");
                return;
            }
            if (isMPM)
            {
                var mods = GetInstalledPackages();
                foreach(var mod in mods)
                {
                    Console.WriteLine(mod);
                }
            }
            if (isModloader)
            {
                var mods = new Dictionary<string, List<string>>();

                //Folders
                GetModsInFolder("cleo/*", "cleo", mods);
                GetModsInFolder("decision/allowed/*", "ped/grp", mods);
                GetModsInFolder("models/txd/*", "sprites", mods);
                GetModsInFolder("player.img/*", "clothing", mods);
                GetModsInFolder("gta3.img/*", "nodes", mods);
                GetModsInFolder("GENRL/*", "audio", mods);

                //Extensions
                GetModsInFolder("*.asi", "scripts", mods);
                GetModsInFolder("*.col", "collision", mods);
                GetModsInFolder("*.txd", "texture", mods);
                GetModsInFolder("*.dff", "model", mods);
                GetModsInFolder("*.ide", "map", mods);
                GetModsInFolder("*.ipl", "map", mods);
                GetModsInFolder("*.ifp", "animation", mods);

                foreach (var mod in mods)
                {
                    Console.WriteLine($"mod#{mod.Key}#{String.Join(", ", mod.Value.ToArray())}");
                }

            }
        }
        public void GetModsInFolder(string search, string mod, Dictionary<string, List<String>> mods)
        {
            var matcher = new Matcher();
            matcher.AddExclude(".data/**");
            matcher.AddInclude($"**/{search}");

            var result = matcher.Execute(
                new DirectoryInfoWrapper(
                    new DirectoryInfo(Path.Combine(Program.fullPath, "modloader"))));

            foreach (var file in result.Files)
            {
                var paths = new List<string>() { "C://", file.Path };
                foreach (var sub in search.Split('/'))
                    paths.Add("../");

                var dir = Path.GetFullPath(Path.Combine(paths.ToArray()));
                dir = dir.Replace("C:\\", string.Empty).TrimEnd('\\');
                if (mods.ContainsKey(dir))
                {
                    if (!mods[dir].Contains(mod))
                        mods[dir].Add(mod);
                }
                else
                {
                    mods.Add(dir, new List<string>() { mod });
                }
            }
        }
    }
}
