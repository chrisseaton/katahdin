using System;
using System.Collections.Generic;

using Gtk;

namespace Katahdin.Debugger
{
    public class NewSessionDialog : Dialog
    {
        private ListStore fileStore;
        private CheckButton importStandard;
        private CheckButton compileParser;
        private CheckButton debugGrammar;
        private CheckButton debugParser;
        private CheckButton debugParseTrees;
        
        public NewSessionDialog(Window parent, CommandLine commandLine)
            : base("New Katahdin Debugger Session", parent, DialogFlags.Modal
                | DialogFlags.DestroyWithParent)
        {
            WindowPosition = WindowPosition.CenterOnParent;
            
            AddActionWidget(new Button(Stock.Cancel), ResponseType.Cancel);
            AddActionWidget(new Button(Stock.Ok), ResponseType.Ok);
            
            VBox v = new VBox();
            v.BorderWidth = 5;
            VBox.PackStart(v, true, true, 0);
            
            ScrolledWindow fileScroller = new ScrolledWindow();
            fileScroller.BorderWidth = 5;
            fileScroller.ShadowType = ShadowType.In;
            v.PackStart(fileScroller, true, true, 0);
            
            fileStore = new ListStore(typeof(string));
            
            TreeView fileView = new TreeView(fileStore);
            fileView.AppendColumn("Input files", new CellRendererText(), "text", 0);
            fileScroller.Add(fileView);
            
            foreach (string file in commandLine.Files)
                fileStore.AppendValues(file);
            
            importStandard = new CheckButton("Import standard library");
            v.PackStart(importStandard, false, true, 0);
            
            importStandard.Active = ! (bool) commandLine.GetOption("-nostd");
            
            compileParser = new CheckButton("Compile parser");
            compileParser.Active = true;
            v.PackStart(compileParser, false, true, 0);
            
            debugGrammar = new CheckButton("Debug grammar");
            v.PackStart(debugGrammar, false, true, 0);
            
            debugParser = new CheckButton("Debug parser");
            v.PackStart(debugParser, false, true, 0);
            
            debugParseTrees = new CheckButton("Debug parse trees");
            v.PackStart(debugParseTrees, false, true, 0);
            
            ShowAll();
        }
        
        public new bool Run()
        {
            return (ResponseType) base.Run() == ResponseType.Ok;
        }
        
        public List<string> Files
        {
            get
            {
                List<string> files = new List<string>();
                
                foreach (object[] row in fileStore)
                    files.Add((string) row[0]);
                
                return files;
            }
        }
        
        public bool ImportStandard
        {
            get
            {
                return importStandard.Active;
            }
        }
        
        public bool CompileParser
        {
            get
            {
                return compileParser.Active;
            }
        }

        public bool DebugGrammar
        {
            get
            {
                return debugGrammar.Active;
            }
        }
    
        public bool DebugParser
        {
            get
            {
                return debugParser.Active;
            }
        }
        
        public bool DebugParseTrees
        {
            get
            {
                return debugParseTrees.Active;
            }
        }
    }
}
