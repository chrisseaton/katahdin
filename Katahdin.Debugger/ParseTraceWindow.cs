using System;
using System.Collections.Generic;

using Gtk;

using Katahdin.Collections;

namespace Katahdin.Debugger
{
    public class ParseTraceWindow : Window
    {
        public event SourceSelectedHandler SourceSelected;
        
        private ScrolledWindow scroller;
        private TreeStore store;
        private TreeView view;
        private TreeViewColumn column;
        
        private System.Collections.Generic.Stack<TreeIter> stack = new System.Collections.Generic.Stack<TreeIter>();
        
        private const int VALUE_TEXT = 0;
        private const int VALUE_SOURCE = 1;
        private const int VALUE_LINK = 2;
        
        private List<ListTupple> updates = new List<ListTupple>();
        
        private Dictionary<object, TreePath> tags = new Dictionary<object, TreePath>();
        
        public ParseTraceWindow(RuntimeThread runtimeThread) : base("Parse Trace")
        {
            runtimeThread.Runtime.ParseTrace.EnterEvent += OnEnter;
            runtimeThread.Runtime.ParseTrace.LeaveEvent += OnLeave;
            
            SetDefaultSize(250, 300);
            SkipPagerHint = true;
            SkipTaskbarHint = true;
            
            scroller = new ScrolledWindow();
            scroller.BorderWidth = 5;
            scroller.ShadowType = ShadowType.In;
            Add(scroller);
            
            store = new TreeStore(typeof(string), typeof(Source), typeof(GtkProofBox));
            
            view = new TreeView(store);
            column = view.AppendColumn(null, new CellRendererText(), "text", VALUE_TEXT);
            view.HeadersVisible = false;
            view.RowActivated += OnTreeRowActivated;
            scroller.Add(view);
            
            GLib.Timeout.Add(50, OnUpdatesTimeout);
            
            // TODO need a context menu for view source and follow link
        }
        
        protected override bool OnDeleteEvent(Gdk.Event args)
        {
            Hide();

            return true;
        }
        
        private void OnEnter(Source source, string label, object tag, object link)
        {
            lock (updates)
            {
                updates.Add(new ListTupple(source, label, tag, link));
            }
        }
        
        private void OnLeave()
        {
            lock (updates)
            {
                updates.Add(null);
            }
        }
        
        private bool OnUpdatesTimeout()
        {
            lock (updates)
            {
                foreach (ListTupple update in updates)
                {
                    if (update == null)
                    {
                        stack.Pop();
                    }
                    else
                    {
                        Source source = (Source) update[0];
                        string label = (string) update[1];
                        object tag = update[2];
                        object link = update[3];
                        
                        GtkProofBox linkBox = null;
                        
                        if (link != null)
                        {
                            linkBox = new GtkProofBox(tags[link]);
                            label = label + " ->";
                        }
                        
                        TreeIter iter;

                        if (stack.Count == 0)
                            iter = store.AppendValues(label, source, linkBox);
                        else
                            iter = store.AppendValues(stack.Peek(), label, source, linkBox);
                        
                        stack.Push(iter);
                        
                        if (tag != null)
                        {
                            TreePath path = store.GetPath(iter);
                            tags[tag] = path;
                        }
                    }
                }
                
                updates.Clear();
            }
            
            return true;
        }

        private void OnTreeRowActivated(object obj, RowActivatedArgs args)
        {
            TreeIter iter;
            store.GetIter(out iter, args.Path);
            
            GtkProofBox linkBox = (GtkProofBox) store.GetValue(iter, VALUE_LINK);
            
            if (linkBox != null)
            {
                TreePath link = (TreePath) linkBox.Contents;
                view.ExpandToPath(link);
                view.SetCursor(link, column, false);
                
                return;
            }
            
            Source source = (Source) store.GetValue(iter, VALUE_SOURCE);

            if ((source != null) && (SourceSelected != null))
                SourceSelected(source, true);
        }
    }
}
