using System;
using System.Collections.Generic;

using Gtk;

using Katahdin.Collections;
using Katahdin.Grammars;

namespace Katahdin.Debugger
{
    public class ParseTreeWindow : Window
    {
        public event SourceSelectedHandler SourceSelected;
        
        public delegate void ObjectViewedHandler(object obj);
        public event ObjectViewedHandler ObjectViewed;
        
        private ScrolledWindow scroller;
        private TreeStore store;
        private TreeView view;
        private TreeViewColumn column;
        
        private Menu menu;
        
        private const int VALUE_TEXT = 0;
        private const int VALUE_OBJECT = 1;
        
        public ParseTreeWindow(RuntimeThread runtimeThread) : base("Parse Tree")
        {
            runtimeThread.Runtime.ParseTrace.ParsedEvent += OnParsed;
            
            SetDefaultSize(250, 300);
            SkipPagerHint = true;
            SkipTaskbarHint = true;
            
            scroller = new ScrolledWindow();
            scroller.BorderWidth = 5;
            scroller.ShadowType = ShadowType.In;
            Add(scroller);
            
            store = new TreeStore(typeof(string), typeof(object));
            
            view = new TreeView(store);
            column = view.AppendColumn(null, new CellRendererText(), "text", VALUE_TEXT);
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
        
        private void OnParsed(string fileName, RuntimeObject obj)
        {
            Application.Invoke(delegate
            {
                TreeIter parent = store.AppendValues(fileName, null);
                Add(parent, "", obj);
                view.SetCursor(store.GetPath(parent), column, false);
            });
        }

        private void OnTreeRowActivated(object sender, RowActivatedArgs args)
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
            object obj = store.GetValue(iter, VALUE_OBJECT);
        
            if ((obj != null) && (obj is RuntimeObject))
            {
                Source source = ((RuntimeObject) obj).Source;
        
                if ((source != null) && (SourceSelected != null))
                    SourceSelected(source, false);
            }
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
        
        private TreeIter Add(TreeIter parent, string prefix, RuntimeObject node)
        {
            Type type = node.GetType();
            parent = store.AppendValues(parent, prefix + TypeNames.GetName(type), node);
            
            // FIXME
            try
            {
            foreach (string field in Pattern.PatternForType(type).Fields)
                Add(parent, field + ": ", type.GetField(field).GetValue(node));
            }
            catch (Exception)
            {
            }
            
            return parent;
        }
        
        private void Add(TreeIter parent, string prefix, List<object> nodes)
        {
            parent = store.AppendValues(parent, prefix + "List", nodes);
            
            foreach (object node in nodes)
                Add(parent, "", node);
        }
        
        private void Add(TreeIter parent, string prefix, string node)
        {
            store.AppendValues(parent, prefix + TextEscape.Quote(node), GtkProofBox.Box(node));
        }

        private void Add(TreeIter parent, string prefix, object node)
        {
            if (node == null)
                store.AppendValues(parent, prefix + "null", node);
            else if (node is RuntimeObject)
                Add(parent, prefix, node as RuntimeObject);
            else if (node is List<object>)
                Add(parent, prefix, node as List<object>);
            else if (node is string)
                Add(parent, prefix, node as string);
            else
                throw new Exception("Unexpected parse tree object class " + node.GetType());
        }
    }
}
