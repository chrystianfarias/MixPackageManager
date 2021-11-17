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

                if (!arg.Contains("@") && !arg.Contains(".json"))
                    arg = $"mods/{arg}@latest.json";

                InstallPackage(arg);
            }
        }
        [Command("check")]
        public void CheckInstalledMod(Arguments arguments)
        {
            if (arguments.Length >= 2)
            {
                var arg = arguments[1];

                if (!arg.Contains("@") && !arg.Contains(".json"))
                    arg = $"mods/{arg}@latest.json";

                CheckInstalledMod(arg);
            }
        }
        public void CheckInstalledMod(string package)
        {
            var packageInfo = package.Split('/')[1].Split('@'); //example: mods/modA@latest.json
            var packageName = packageInfo[0];  //example: modA
            var packageVersion = packageInfo[1].Split('.')[0]; //example: latest
            var mods = GetInstalledPackages();
            var existentPackage = mods.FirstOrDefault(mod =>
            {
                var modInfo = mod.Split('/')[1].Split('@');
                var modName = modInfo[0];
                var modVersion = modInfo[1].Split('.')[0];

                return modName == packageName && modVersion == packageVersion;
            });
            if (existentPackage != null)
                Console.WriteLine(Program.isGui ? $"package#true" : $"Package {packageName} with version {packageVersion} is installed");
            else
                Console.WriteLine(Program.isGui ? $"package#false" : $"Package {package} is not installed");
        }
        public void InstallPackage(string package)
        {
            if (!File.Exists("mods.json"))
            { 
                Program.Error("mods.json not found! Run 'mpm init' first");
                return;
            }
            var mods = GetInstalledPackages();
            var packageInfo = package.Split('/')[1].Split('@'); //example: mods/modA@latest.json
            var packageName = packageInfo[0];  //example: modA
            var packageVersion = packageInfo[1].Split('.')[0]; //example: latest

            mods.FirstOrDefault(mod =>
            {
                var modInfo = mod.Split('/')[1].Split('@');
                var modName = modInfo[0];
                var modVersion = modInfo[1].Split('.')[0];

                return modName == packageName && modVersion == packageVersion;
            }); //Is installed

            try
            {
                var installed = false;
                var client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);

                client.DownloadStringAsync(new Uri($"{Program.API}{package}"));
                client.DownloadStringCompleted += (s, e) =>
                {
                    var jsonString = e.Result;
                    var mod = JsonConvert.DeserializeObject<Mod>(jsonString);

                    //Teste!!
                    mod.Url = "https://beta.mixmods.com.br/launcher/mods/files/SA%20-%20SilentPatch.7z";
                    Console.WriteLine(mod.Url);

                    client.DownloadDataAsync(new Uri(mod.Url));
                    client.DownloadDataCompleted += (s2, e2) =>
                    {
                        var modFile = e2.Result;
                        var modStream = new MemoryStream(modFile);

                        using (ArchiveFile archiveFile = new ArchiveFile(modStream))
                        {
                            Console.WriteLine(Program.isGui ? $"extract#" : $"Extracting {packageName} package");
                            archiveFile.Extract(Program.fullPath);
                        }

                        Console.WriteLine(Program.isGui ? $"install#" : $"Installing {packageName} package");
                        //TODO: installation

                        mods.Add(package);
                        SetInstalledPackages(mods);

                        Console.WriteLine(Program.isGui ? $"complete" : $"Package {packageName} Installed");
                        installed = true;
                    };
                };
                while(installed == false)
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
                        return;
                    }
                    if (resp.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        Program.Error("Server Error!");
                        return;
                    }
                    Program.Error(resp.StatusDescription);
                    return;
                }
                Program.Error(ex.Message);
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
            if (File.Exists("mods.json"))
            {
                var modsText = File.ReadAllText("mods.json");
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
            if (File.Exists("gta_sa.exe"))
            {
                File.WriteAllText("mods.json", "[]");
            }
            else
            {
                Program.Error("run command in GTA SA root folder!");
            }
        }
    }
}
