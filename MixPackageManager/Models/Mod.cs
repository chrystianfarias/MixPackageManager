using Newtonsoft.Json;

namespace MixMods.MixPackageManager.Models
{
    public class Mod
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("dependency_mods")]
        public string[] DependencyMods { get; set; }
    }
}
