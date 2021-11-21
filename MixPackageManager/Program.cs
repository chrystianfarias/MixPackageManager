using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MixMods.MixPackageManager
{
    public class Program
    {
        //public static string API = "https://beta.mixmods.com.br/launcher/";
        public static string API = "https://raw.githubusercontent.com/chrystianfarias/mpm/main/";
        public static Arguments arguments;
        public static string fullPath;
        public static bool isGui;
        private static void Main(string[] args)
        {
            fullPath = System.IO.Directory.GetCurrentDirectory();
            try
            {
                arguments = new Arguments(args);
                isGui = arguments.Contains("-gui");
                //var packageManager = new PackageManager();
                //packageManager.ReorderMods(arguments);
                ProcessCommand();
            }
            catch(Exception ex)
            {
                Error(ex.Message);
                Error(ex.StackTrace);
            }
        }
        private static void ProcessCommand()
        {
            if (arguments.Length == 0)
            {
                Help.Print(arguments);
                return;
            }
            var command = arguments[0];
            var packageManager = new PackageManager();
            
            var methods = AppDomain.CurrentDomain.GetAssemblies() // Returns all currenlty loaded assemblies
                .SelectMany(x => x.GetTypes()) // returns all types defined in this assemblies
                .Where(x => x.IsClass) // only yields classes
                .SelectMany(x => x.GetMethods()) // returns all methods defined in those classes
                .Where(x => x.GetCustomAttributes(typeof(CommandAttribute), false).FirstOrDefault(attr => ((CommandAttribute)attr).cmd == command) != null); // returns only methods that have the InvokeAttribute
            
            foreach (var method in methods) // iterate through all found methods
            {
                var obj = Activator.CreateInstance(method.DeclaringType); // Instantiate the class
                method.Invoke(obj, new object[] { arguments }); // invoke the method
                return;
            }
            Error($"Command '{command}' not found!");
            Help.Print(arguments);
        }
        public static void Error(string msg)
        {
            Console.WriteLine(Program.isGui ? "error#" + msg : "[ERROR] " + msg);
        }
    }
}
