using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using MixMods.MixPackageManager.Models;
using MixMods.MixPackageManager.Utils;
using Newtonsoft.Json;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace MixMods.MixPackageManager
{
    public class PackageManager
    {
        [Command("install")]
        public void InstallPackage(Arguments arguments)
        {
            try
            {
                if (arguments.Length < 2)
                {
                    InstallPackageJson(arguments);
                    return;
                }
                else
                {
                    var arg = arguments[1];
                    var args = arg.Split('@');

                    InstallPackage(args[0], args.Length > 1 ? args[1] : "latest");
                }
            }
            catch(Exception ex)
            {
                Program.Error(ex.Message);
            }
        }
        [Command("check-package")]
        public void CheckInstalledMod(Arguments arguments)
        {
            try
            {
                if (arguments.Length >= 2)
                {
                    var arg = arguments[1];
                    var args = arg.Split('@');

                    CheckInstalledMod(args[0], args.Length > 1 ? args[1] : "latest");
                }
            }
            catch(Exception ex)
            {
                Program.Error(ex.Message);
            }
        }
        public bool CheckInstalledMod(string package, string version)
        {
            var mods = GetInstalledPackages();
            var existentPackage = mods.FirstOrDefault(mod =>
            {
                return mod.Package == package && mod.Version == version;
            });
            if (existentPackage != null)
                Console.WriteLine(Program.isGui ? $"package#true" : $"Package {package} with version {version} is installed");
            else
                Console.WriteLine(Program.isGui ? $"package#false" : $"Package {package} is not installed");
            return existentPackage != null;
        }
        public void InstallPackage(string package, string version)// = "latest")
        {
            if (!File.Exists(Path.Combine(Program.fullPath, "mods.json")))
            { 
                Program.Error("mods.json not found! Run 'mpm init' first");
                return;
            }

            byte[] jsonByte = Download(new Uri($"{Program.API}mods/{package}@{version}.json"));
            if (jsonByte.Length == 0)
                jsonByte = Download(new Uri($"{Program.API}mods/{package}@latest.json"));

            var jsonString = System.Text.Encoding.Default.GetString(jsonByte);
            var mod = JsonConvert.DeserializeObject<Mod>(jsonString);
            mod.Package = package;

            //Install dependencies
            foreach (var dependency in mod.DependencyMods)
            {
                var dependencyPackage = dependency.Split('@');
                InstallPackage(dependencyPackage[0], dependencyPackage[1]);
            }
                    
            var modFile = Download(new Uri(mod.Url));
            var modStream = new MemoryStream(modFile);
            var rootExtractModLoaderFolder = mod.ToDirectory.Replace("{package}", package);
            var extractModLoaderFolder = Path.Combine(Program.fullPath, rootExtractModLoaderFolder);
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
                    Console.WriteLine(Program.isGui ? $"extract#" : $"Extracting {package} package");
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
                File.WriteAllText(Path.Combine(extractModLoaderFolder, $"{package}.mod"), JsonConvert.SerializeObject(mod));
                            
            }
            catch (IOException ex)
            {
                Program.Error("Execute with administrator!");
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(2);
            }

            Console.WriteLine(Program.isGui ? $"install#" : $"Installing {package} package");
            //TODO: installation
            
            AddInstalledPackages(Path.Combine(rootExtractModLoaderFolder, $"{package}.mod"));

            Console.WriteLine(Program.isGui ? $"complete" : $"Package {package} Installed");
        }
        private static byte[] Download(Uri uri)
        {
            Console.WriteLine("Download " + uri.ToString());
            byte[] downloaded = null;
            try
            {
                var client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                client.DownloadDataAsync(uri);
                client.DownloadDataCompleted += (s, e) =>
                {
                    try
                    {
                        downloaded = e.Result;
                    }
                    catch
                    {
                        downloaded = new byte[0];
                    }
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
            catch
            {
                return null;
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
        public void AddInstalledPackages(string modPath)
        {
            var modsText = File.ReadAllText(Path.Combine(Program.fullPath, "mods.json"));
            var modsList = JsonConvert.DeserializeObject<List<string>>(modsText);
            if (!modsList.Contains(modPath))
                modsList.Add(modPath);
            var json = JsonConvert.SerializeObject(modsList);
            File.WriteAllText("mods.json", json);
        }
        public List<Mod> GetInstalledPackages()
        {
            if (File.Exists(Path.Combine(Program.fullPath, "mods.json")))
            {
                var modsText = File.ReadAllText(Path.Combine(Program.fullPath, "mods.json"));
                var modsList = JsonConvert.DeserializeObject<List<string>>(modsText);
                var mods = new List<Mod>();
                foreach(var modPath in modsList)
                {
                    var modText = File.ReadAllText(modPath);
                    var mod = JsonConvert.DeserializeObject<Mod>(modText);
                    mods.Add(mod);
                }
                return mods;
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
        [Command("mod")]
        public void IgnoreMod(Arguments arguments)
        {
            if (arguments.Length >= 3)
            {
                var arg = arguments[1];
                var base64 = Convert.FromBase64String(arg);
                var decodedString = Encoding.Default.GetString(base64);

                var ini = new IniFile(Path.Combine(Program.fullPath, "modloader", "modloader.ini"));
                if (arguments.Contains("-ignore"))
                    ini.Write(decodedString, null, "Profiles.Default.IgnoreMods");
                if (arguments.Contains("-noignore"))
                    ini.DeleteKey(decodedString, "Profiles.Default.IgnoreMods");
            }
        }
        [Command("reorder")]
        public void ReorderMods(Arguments arguments)
        {
            if (arguments.Length >= 2)
            {
                var arg = arguments[1];
                var base64 = Convert.FromBase64String(arg);
                var decodedString = Encoding.Default.GetString(base64);
                var json = JsonConvert.DeserializeObject<string[]>(decodedString);
                
                var ini = new IniFile(Path.Combine(Program.fullPath, "modloader", "modloader.ini"));
                ini.DeleteSection("Profiles.Default.Priority");
                
                //TODO: fix init.Write
                var iniString = "[Profiles.Default.Priority]\n";
                var priority = 1;
                foreach (var mod in json)
                {
                    if (mod == "")
                        continue;
                    //ini.Write(mod, priority.ToString(), "Profiles.Default.Priority");
                    iniString += $"{mod} = {priority}\n";
                    priority++;
                }
                ini.WriteText(iniString);
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
