using MixMods.MixPackageManager.Models;
using Newtonsoft.Json;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace MixMods.MixPackageManager
{
    public class PackageManager
    {
        [Command("install")]
        public void Install(Arguments arguments)
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
                using (var client = new WebClient())
                {
                    var jsonString = client.DownloadString($"{Program.API}{package}");
                    var mod = JsonConvert.DeserializeObject<Mod>(jsonString);

                    var modFile = client.DownloadData(mod.Url);
                    var modStream = new MemoryStream(modFile);

                    using (ArchiveFile archiveFile = new ArchiveFile(modStream))
                    {
                        archiveFile.Extract(Program.fullPath);
                    }

                    mods.Add(package);
                    SetInstalledPackages(mods);
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
        public void InitPackageJson(Arguments arguments)
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
