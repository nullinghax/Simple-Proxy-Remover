using System;
using dnlib.DotNet.Writer;
using dnlib.DotNet;

namespace Proxy_Remover
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = string.Empty;
            if (args.Length > 0)
                path = args[0];
            else
            {
                Console.Write("Path : ");
                path = Console.ReadLine();
            }
            var module = ModuleDefMD.Load(path);
            Remove.Run(module);
            var options = new ModuleWriterOptions(module);
            options.Logger = DummyLogger.NoThrowInstance;
            module.Write(path.Replace(".exe", "-Deobfuscated.exe"), options);
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
