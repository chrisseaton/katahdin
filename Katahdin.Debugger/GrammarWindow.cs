using System;
using System.Collections.Generic;

using Gtk;

using Katahdin.Grammars;
using Katahdin.Grammars.Alts;
using Katahdin.Debugger.ObjectViewer;

namespace Katahdin.Debugger
{
    public class GrammarWindow : Window
    {
        public event SourceSelectedHandler SourceSelected;
        
        public delegate void ObjectViewedHandler(object obj);
        public event ObjectViewedHandler ObjectViewed;
        
        private ScrolledWindow scroller;
        private TreeStore store;
        private TreeView view;
        
        private Menu menu;
        
        private const int VALUE_TEXT = 0;
        private const int VALUE_OBJECT = 1;
        
        private Signal signal = new Signal();
        
        private Dictionary<Pattern, TreePath> patternPaths = new Dictionary<Pattern, TreePath>();
        
        public GrammarWindow(RuntimeThread runtimeThread) : base("Grammar")
        {
            runtimeThread.Runtime.Grammar.Trace.PatternDefinedEvent += OnGrammarPatternDefined;
            runtimeThread.Runtime.Grammar.Trace.PatternChangedEvent += OnGrammarPatternChanged;
            
            SetDefaultSize(250, 300);
            SkipPagerHint = true;
            SkipTaskbarHint = true;
            
            scroller = new ScrolledWindow();
            scroller.BorderWidth = 5;
            scroller.ShadowType = ShadowType.In;
            Add(scroller);
            
            store = new TreeStore(typeof(string), typeof(Source));
            
            // FIXME - only want to sort top level
            
            //TreeModelSort storeSort = new TreeModelSort(store);
            //storeSort.SetSortColumnId(0, SortType.Ascending);
            
            view = new TreeView(store);
            
            view.AppendColumn(null, new CellRendererText(), "text", VALUE_TEXT);
            view.HeadersVisible = false;
            view.RowActivated += OnTreeRowActivated;
            view.ButtonPressEvent += OnTreeButtonPress;
            scroller.Add(view);
            
            MenuBuilder menuBuilder = new MenuBuilder();
            menu = menuBuilder.StartMenu();
            menuBuilder.Add("Show source", OnMenuShowSource);
            menuBuilder.Add("Show object", OnMenuShowObject);
            menu.ShowAll();
        }

        protected override bool OnDeleteEvent(Gdk.Event args)
        {
            Hide();

            return true;
        }
        
        private void OnGrammarPatternDefined(Pattern pattern)
        {
            Application.Invoke(delegate
            {
                string name = TypeNames.GetName(pattern.Type);
            
                if (pattern is AbstractPattern)
                    name += " (abstract)";
                
                TreeIter iter = store.AppendValues(name, pattern);
                AddNode(iter, pattern.ParseGraph);
                
                patternPaths[pattern] = store.GetPath(iter);
                
                signal.Send();
            });
            
            signal.WaitFor();
        }
        
        private void OnGrammarPatternChanged(Pattern pattern)
        {
            Application.Invoke(delegate
            {
                TreeIter iter;
                store.GetIter(out iter, patternPaths[pattern]);
                
                RemoveChildren(iter);
                AddNode(iter, pattern.ParseGraph);
                
                signal.Send();
            });
            
            signal.WaitFor();
        }

        private void OnTreeRowActivated(object obj, RowActivatedArgs args)
        {
            TreeIter iter;
            store.GetIter(out iter, args.Path);
            ShowSource(iter);
        }
        
        [GLib.ConnectBefore]
        private void OnTreeButtonPress(object sender, ButtonPressEventArgs args)
        {
            if (args.Event.Button == 3)
                menu.Popup();
        }
        
        private void OnMenuShowSource(object obj, EventArgs args)
        {
            TreeIter iter;
            view.Selection.GetSelected(out iter);
            ShowSource(iter);
        }
        
        private void ShowSource(TreeIter iter)
        {
            ISourced sourced = (ISourced) store.GetValue(iter, VALUE_OBJECT);
            Source source = sourced.Source;
            
            if ((source != null) && (SourceSelected != null))
                SourceSelected(source, false);
        }
        
        private void OnMenuShowObject(object obj, EventArgs args)
        {
            TreeIter iter;
            view.Selection.GetSelected(out iter);
            
            object viewed = store.GetValue(iter, VALUE_OBJECT);
            
            viewed = GtkProofBox.Unbox(viewed);
            
            if (ObjectViewed != null)
                ObjectViewed(viewed);
        }
        
        private void RemoveChildren(TreeIter parent)
        {
            TreeIter child;
            
            if (store.IterChildren(out child, parent))
            {
                do
                    RemoveChildren(child);
                while (store.Remove(ref child));
            }
        }
        
        private void AddNode(TreeIter parent, IParseable node)
        {
            if (node is ParseGraphNode)
                AddNode(parent, (ParseGraphNode) node);
            else if (node is Alt)
                AddNode(parent, (Alt) node);
            else
                throw new Exception("Unsupported grammar node type " + node.GetType() + " in viewer");
        }
        
        private void AddNode(TreeIter parent, ParseGraphNode node)
        {
            if (node is AnyNode)
                AddNode(parent, (AnyNode) node);
            else if (node is TextNode)
                AddNode(parent, (TextNode) node);
            else if (node is CharNode)
                AddNode(parent, (CharNode) node);
            else if (node is PatternNode)
                AddNode(parent, (PatternNode) node);
            else if (node is AltNode)
                AddNode(parent, (AltNode) node);
            else if (node is RepNode)
                AddNode(parent, (RepNode) node);
            else if (node is SeqNode)
                AddNode(parent, (SeqNode) node);
            else if (node is TokenNode)
                AddNode(parent, (TokenNode) node);
            else if (node is LabelNode)
                AddNode(parent, (LabelNode) node);
            else if (node is OptionsNode)
                AddNode(parent, (OptionsNode) node);
            else if (node is FailNode)
                AddNode(parent, (FailNode) node);
            else if (node is AndNode)
                AddNode(parent, (AndNode) node);
            else if (node is NotNode)
                AddNode(parent, (NotNode) node);
            else if (node is UserDefinedNode)
                AddNode(parent, (UserDefinedNode) node);
            else
                throw new Exception("Unsupported grammar node type " + node.GetType() + " in viewer");
        }
        
        private void AddNode(TreeIter parent, AnyNode anyNode)
        {
            store.AppendValues(parent, "[]", anyNode);
        }
        
        private void AddNode(TreeIter parent, TextNode textNode)
        {
            store.AppendValues(parent, TextEscape.Quote(textNode.Text), textNode);
        }

        private void AddNode(TreeIter parent, CharNode charNode)
        {
            store.AppendValues(parent, charNode.Range.ToString(), charNode);
        }

        private void AddNode(TreeIter parent, PatternNode patternNode)
        {
            store.AppendValues(parent, patternNode.Pattern.Type.Name, patternNode);
        }

        private void AddNode(TreeIter parent, AltNode alt)
        {
            parent = store.AppendValues(parent, "alt", alt);
            AddNode(parent, alt.Implementation);
        }
        
        private void AddNode(TreeIter parent, RepNode rep)
        {
            parent = store.AppendValues(parent, rep.Reps.Name, rep);
            AddNode(parent, rep.Body);
        }
        
        private void AddNode(TreeIter parent, SeqNode seq)
        {
            parent = store.AppendValues(parent, "seq", seq);
            
            foreach (ParseGraphNode node in seq.Nodes)
                AddNode(parent, node);
        }
        
        private void AddNode(TreeIter parent, TokenNode token)
        {
            parent = store.AppendValues(parent, "token", token);
            AddNode(parent, token.Body);
        }

        private void AddNode(TreeIter parent, LabelNode label)
        {
            parent = store.AppendValues(parent, label.Label + ":", label);
            AddNode(parent, label.Body);
        }
        
        private void AddNode(TreeIter parent, OptionsNode optionsNode)
        {
            parent = store.AppendValues(parent, "options", optionsNode);
            
            Dictionary<string, object> options = new Dictionary<string, object>();
            
            if (optionsNode.BuildTextNodes.HasValue)
                options["buildTextNodes"] = optionsNode.BuildTextNodes.Value;

            if (optionsNode.RecursionBehaviour.HasValue)
                options["recursionBehaviour"] = optionsNode.RecursionBehaviour.Value;

            if (optionsNode.DropPrecedence.HasValue)
                options["dropPrecedence"] = optionsNode.DropPrecedence.Value;

            if (optionsNode.Whitespace.HasValue)
                options["whitespace"] = optionsNode.Whitespace.Value;

            if (optionsNode.Exclude.HasValue)
                options["exclude"] = optionsNode.Exclude.Value;
            
            foreach (KeyValuePair<string, object> option in options)
                store.AppendValues(parent, option.Key + " = "
                    + ObjectViewerDirectory.GetDescription(option.Value), optionsNode);
            
            AddNode(parent, optionsNode.Body);
        }
        
        private void AddNode(TreeIter parent, FailNode fail)
        {
            store.AppendValues(parent, fail.Message, fail);
        }
        
        private void AddNode(TreeIter parent, AndNode and)
        {
            parent = store.AppendValues(parent, "&", and);
            AddNode(parent, and.Body);
        }
        
        private void AddNode(TreeIter parent, NotNode not)
        {
            parent = store.AppendValues(parent, "!", not);
            AddNode(parent, not.Body);
        }

        private void AddNode(TreeIter parent, UserDefinedNode userDefinedNode)
        {
            store.AppendValues(parent, "user defined", userDefinedNode);
        }

        private void AddNode(TreeIter parent, Alt alt)
        {
            if (alt is LongestAlt)
                AddNode(parent, (LongestAlt) alt);
            else if (alt is PrecedenceAlt)
                AddNode(parent, (PrecedenceAlt) alt);
            else
                throw new Exception("Unsupported grammar node type "
                    + alt.GetType() + " in viewer");
        }

        private void AddNode(TreeIter parent, LongestAlt longestAlt)
        {
            parent = store.AppendValues(parent, "longest alt", longestAlt);
            
            foreach (IParseable alt in longestAlt.Alts)
                AddNode(parent, alt);
        }

        private void AddNode(TreeIter parent, PrecedenceAlt precedenceAlt)
        {
            parent = store.AppendValues(parent, "precedence alt", precedenceAlt);

            foreach (PrecedenceAltGroup group in precedenceAlt.Groups)
            {
                TreeIter groupParent = store.AppendValues(parent, "group " + group.Precedence, group);
                AddNode(groupParent, group.ParseGraph);
            }
        }
    }
}
