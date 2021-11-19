using Newtonsoft.Json;

namespace MixMods.MixPackageManager.Models
{
    public class Mod
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
        
        [JsonProperty("extract_dir")]
        public string ExtractDir { get; set; }

        [JsonProperty("to_directory")]
        public string ToDirectory { get; set; }

        [JsonProperty("dependency_mods")]
        public string[] DependencyMods { get; set; }
    }
}
