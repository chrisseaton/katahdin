using System;
using System.Collections.Generic;
using System.Reflection;

namespace Katahdin.Interpreter
{
    public static class EntryPoint
    {
        public static void Main(string[] args)
        {
            try
            {
                CommandLine commandLine = new CommandLine();
                commandLine.AddOption("Verbose", false, "-verbose");
                
                commandLine.Parse(args);
            
                if ((bool) commandLine.GetOption("-h"))
                {
                    commandLine.Help("Katahdin Interpreter");
                    return;
                }
                
                Runtime runtime = new Runtime(true, false, false, false,
                    (bool) commandLine.GetOption("-verbose"));
                
                //new ConsoleParseTrace(runtime);
                
                runtime.SetUp(commandLine.Args);
                
                if (!((bool) commandLine.GetOption("-nostd")))
                    runtime.ImportStandard();
                
                foreach (string file in commandLine.Files)
                    runtime.Import(file);
            }
            catch (Exception e)
            {
                /*while (true)
                {
                    TargetInvocationException wrapper
                        = e as TargetInvocationException;

                    if (wrapper == null)
                        break;

                    e = wrapper.InnerException;
                }*/
                
                Console.Error.WriteLine(e);
            }
        }
    }
}
