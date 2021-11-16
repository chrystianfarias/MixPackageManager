using System.Collections.Generic;
using System.Linq;

namespace MixMods.MixPackageManager
{
    public class Arguments
    {
        private List<string> args;
        public Arguments(string[] args)
        {
            this.args = new List<string>(args);
        }
        public int IndexOf(string key)
        {
            return args.FindIndex(k => k == key);
        }
        public bool Contains(string key)
        {
            return args.FirstOrDefault(k => k == key) != null;
        }
        public string this[int index]
        {
            get => args[index];
        }
        public int Length
        {
            get => args.Count;
        }
    }
}
