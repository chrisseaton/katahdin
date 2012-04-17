using System;
using System.Collections.Generic;

using Gtk;

namespace Katahdin.Debugger
{
    public static class EntryPoint
    {
        public static void Main(string[] args)
		{
            CommandLine commandLine = new CommandLine();
            commandLine.Parse(args);
            
            if ((bool) commandLine.GetOption("-h"))
            {
                commandLine.Help("Katahdin Graphical Debugger");
                return;
            }
            
	        Application.Init();
			
			MainWindow mainWindow = new MainWindow();
			mainWindow.ShowAll();
			
			if (mainWindow.NewSession(commandLine))
			    Application.Run();
			
			mainWindow.Destroy();
		}
    }
}
