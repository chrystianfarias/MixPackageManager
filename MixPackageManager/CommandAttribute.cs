namespace MixMods.MixPackageManager
{
    [System.AttributeUsage(System.AttributeTargets.Method)
]
    public class CommandAttribute : System.Attribute
    {
        public string cmd;

        public CommandAttribute(string cmd)
        {
            this.cmd = cmd;
        }
    }
}
