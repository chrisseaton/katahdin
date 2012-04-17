using System;
using System.Collections.Generic;

using Gtk;

namespace Katahdin.Debugger
{
    public class MainWindow : Window
    {
        private List<string> files;
        private RuntimeThread runtimeThread;
        
        private GrammarWindow grammarWindow;
        private ParseTraceWindow parseTraceWindow;
        private ParseTreeWindow parseTreeWindow;
        
        private MenuItem debugRun;
        private CheckMenuItem viewGrammar;
        private CheckMenuItem viewParseTrace;
        private CheckMenuItem viewParseTree;
        
        private ConsoleWidget console;
        
        private ProgressBar progress;
        
        private Label statusLabel;
        private bool showingErrorInStatus;
        
        private Dictionary<string, FileWindow> fileWindows
            = new Dictionary<string, FileWindow>();
        
        public MainWindow()
            : base("Katahdin Debugger")
        {
            SetDefaultSize(500, 400);
            
            try
            {
                PathResolver pathResolver = new PathResolver();
                SetIconFromFile(pathResolver.Resolve("katahdin.svg"));
            }
            catch
            {
            }
            
            Destroyed += delegate
            {
                if (runtimeThread != null)
                    runtimeThread.Shutdown();
            };
            
            VBox vertical = new VBox();
            Add(vertical);
            
            MenuBuilder menuBuilder = new MenuBuilder();
            
            MenuBar menuBar = menuBuilder.StartMenuBar();
            vertical.PackStart(menuBar, false, false, 0);
            
            menuBuilder.StartMenu("Debug");
            debugRun = menuBuilder.Add("Run", OnDebugRun);
            menuBuilder.End();
            
            menuBuilder.StartMenu("View");
            viewGrammar = menuBuilder.AddCheck("Grammar", OnViewGrammarToggled);
            viewParseTrace = menuBuilder.AddCheck("Parse Trace", OnViewParseTraceToggled);
            viewParseTree = menuBuilder.AddCheck("Parse Tree", OnViewParseTreeToggled);
            menuBuilder.Separate();
            menuBuilder.Add("View runtime object", OnViewRuntimeModule);
            menuBuilder.End();
            
            menuBuilder.End();
            
            console = new ConsoleWidget();
            vertical.PackStart(console, true, true, 0);
            
            vertical.PackStart(new HSeparator(), false, false, 0);
            
            HBox statusBar = new HBox();
            vertical.PackStart(statusBar, false, false, 1);
            
            progress = new ProgressBar();
            statusBar.PackStart(progress, false, false, 1);
            
            statusLabel = new Label();
            statusLabel.SetAlignment(0, (float) 0.5);
            statusLabel.LineWrap = true;
            statusBar.PackStart(statusLabel, true, true, 0);
        }
        
        public bool NewSession(CommandLine commandLine)
        {
            NewSessionDialog newSessionDialog = new NewSessionDialog(this, commandLine);
            
            bool response = newSessionDialog.Run();
            
            if (response)
            {
                if (runtimeThread != null)
                    runtimeThread.Shutdown();
                
                Runtime runtime = new Runtime(
                    newSessionDialog.CompileParser,
                    newSessionDialog.DebugParser,
                    newSessionDialog.DebugParseTrees,
                    newSessionDialog.DebugGrammar,
                    true);
                
                runtimeThread = new RuntimeThread(runtime);
                runtimeThread.ImportStandard = newSessionDialog.ImportStandard;
                
                files = newSessionDialog.Files;

                runtimeThread.RunningEvent += OnRuntimeThreadRunning;
                runtimeThread.FinishedEvent += OnRuntimeThreadFinished;
                runtimeThread.RuntimeErrorEvent += OnRuntimeError;
                
                viewParseTrace.Visible = runtime.TraceParser;
                
                if (runtime.TraceParser)
                {
                    runtime.ParseTrace.ProgressChangedEvent += OnProgressChanged;
                    
                    parseTraceWindow = new ParseTraceWindow(runtimeThread);
                    parseTraceWindow.TransientFor = this;
                    
                    parseTraceWindow.SourceSelected += OnSourceSelected;
                    parseTraceWindow.Hidden += delegate { viewParseTrace.Active = false; };
                }
                
                viewParseTree.Visible = runtime.TraceParseTrees;
                
                if (runtime.TraceParseTrees)
                {
                    parseTreeWindow = new ParseTreeWindow(runtimeThread);
                    parseTreeWindow.TransientFor = this;
                    
                    parseTreeWindow.SourceSelected += OnSourceSelected;
                    parseTreeWindow.ObjectViewed += OnObjectViewed;
                    parseTreeWindow.Hidden += delegate { viewParseTree.Active = false; };
                }
                
                viewGrammar.Visible = runtime.TraceGrammar;
                
                if (runtime.TraceGrammar)
                {
                    grammarWindow = new GrammarWindow(runtimeThread);
                    grammarWindow.TransientFor = this;

                    grammarWindow.SourceSelected += OnSourceSelected;
                    grammarWindow.ObjectViewed += OnObjectViewed;
                    grammarWindow.Hidden += delegate { viewGrammar.Active = false; };
                }
            }
            
            newSessionDialog.Destroy();
            return response;
        }

        protected override bool OnDeleteEvent(Gdk.Event args)
        {
            if (base.OnDeleteEvent(args))
                return true;
            
            Application.Quit();
            
            return false;
        }
        
        private void OnRuntimeThreadRunning()
        {
            Application.Invoke(delegate
            {
                debugRun.Sensitive = false;
                
                statusLabel.Text = "Running";
                showingErrorInStatus = false;
            });
        }

        private void OnRuntimeThreadFinished()
        {
            Application.Invoke(delegate
            {
                progress.Fraction = 0;
                
                if (!showingErrorInStatus)
                    statusLabel.Text = "Finished";
            });
        }
        
        private void OnRuntimeError(Exception exception)
        {
            if (exception is ParseException)
            {
                Application.Invoke(delegate
                {
                    ParseException parseException = (ParseException) exception;
                    
                    statusLabel.Text = parseException.Message;
                    showingErrorInStatus = true;
                    
                    Source source = parseException.CodeSource;
                    FileWindow window = ShowFileWindow(source.FileName);
                    window.FileView.Highlight(source, true);
                });
            }
            else
            {
                Console.Error.WriteLine(exception);
            }
        }

        private void OnProgressChanged(Lexer lexer)
        {
            if (lexer.Progress - progress.Fraction < 0.01)
                return;
            
            Application.Invoke(delegate
            {
                progress.Fraction = lexer.Progress;
            });
        }
        
        private void OnDebugRun(object obj, EventArgs args)
        {
            runtimeThread.Run(files);
        }
        
        private void OnViewGrammarToggled(object obj, EventArgs args)
        {
            if (viewGrammar.Active)
                grammarWindow.ShowAll();
            else
                grammarWindow.Hide();
        }

        private void OnViewParseTraceToggled(object obj, EventArgs args)
        {
            if (viewParseTrace.Active)
                parseTraceWindow.ShowAll();
            else
                parseTraceWindow.Hide();
        }
        
        private void OnViewParseTreeToggled(object obj, EventArgs args)
        {
            if (viewParseTree.Active)
                parseTreeWindow.ShowAll();
            else
                parseTreeWindow.Hide();
        }
        
        private void OnViewRuntimeModule(object obj, EventArgs args)
        {
            ObjectWindow viewer = new ObjectWindow(runtimeThread, runtimeThread.Runtime);
            viewer.TransientFor = this;
            viewer.ShowAll();
        }

        private void OnSourceSelected(Source source, bool highlightUpTo)
        {
            FileWindow window = ShowFileWindow(source.FileName);
            window.FileView.Highlight(source, highlightUpTo);
        }
        
        private FileWindow ShowFileWindow(string fileName)
        {
            FileWindow window;
            
            if (fileWindows.ContainsKey(fileName))
            {
                window = fileWindows[fileName];
                window.Present();
                return window;
            }
            
            window = new FileWindow(fileName);
            window.TransientFor = this;
            
            fileWindows[fileName] = window;
            
            window.ShowAll();
            
            return window;
        }
        
        private void OnObjectViewed(object obj)
        {
            ObjectWindow viewer = new ObjectWindow(runtimeThread, obj);
            viewer.TransientFor = this;
            viewer.ShowAll();
        }
    }
}
