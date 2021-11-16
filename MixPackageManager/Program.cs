using System;
using System.Collections.Generic;
using System.Linq;

namespace MixMods.MixPackageManager
{
    public class Program
    {
        public static Arguments arguments;
        private static void Main(string[] args)
        {
            try
            {
                arguments = new Arguments(args);
                ProcessCommand();
            }
            catch(Exception ex)
            {
                Error(ex.Message);
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
            Console.WriteLine("[ERROR] " + msg);
        }
    }
}
