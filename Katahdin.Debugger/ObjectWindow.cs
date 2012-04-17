using System;
using System.Reflection;
using System.Collections.Generic;

using Gtk;

using Katahdin.Collections;
using Katahdin.Grammars;
using Katahdin.Debugger.ObjectViewer;

namespace Katahdin.Debugger
{
    public class ObjectWindow : Window
    {
        private ScrolledWindow scroller;
        private TreeStore store;
        private TreeView view;
        
        private const int VALUE_TEXT = 0;
        private const int VALUE_OBJECT = 1;
        private const int VALUE_RAW = 2;
        
        public ObjectWindow(RuntimeThread runtimeThread, object root)
            : base(GetTitle(root))
        {
            SetDefaultSize(250, 300);
            SkipPagerHint = true;
            SkipTaskbarHint = true;
            
            scroller = new ScrolledWindow();
            scroller.BorderWidth = 5;
            scroller.ShadowType = ShadowType.In;
            Add(scroller);
            
            store = new TreeStore(typeof(string), typeof(object), typeof(bool));
            
            //TreeModelSort storeSort = new TreeModelSort(store);
            //storeSort.SetSortColumnId(0, SortType.Ascending);
            
            view = new TreeView(store);
            view.AppendColumn(null, new CellRendererText(), "text", VALUE_TEXT);
            view.HeadersVisible = false;
            view.RowExpanded += OnTreeRowExpanded;
            view.RowCollapsed += OnTreeRowCollapsed;
            scroller.Add(view);
            
            Add(false, new TreeIter(), "", root, false, false);
            
            TreeIter first;
            store.GetIterFirst(out first);
            view.ExpandRow(store.GetPath(first), false);
        }
        
        private void Add(bool hasParent, TreeIter parent, string prefix, object obj, bool loadMembers, bool raw)
        {
            IObjectViewer viewer;
            
            if (raw)
                viewer = ObjectViewerDirectory.Standard;
            else
                viewer = ObjectViewerDirectory.Choose(obj);
            
            ObjectViewHeader header = viewer.GetHeader(obj);
            
            if (!loadMembers)
            {
                if (hasParent)
                    parent = store.AppendValues(parent, prefix + header.Description, GtkProofBox.Box(obj), false);
                else
                    parent = store.AppendValues(prefix + header.Description, GtkProofBox.Box(obj), false);
            }
            
            if (header.HasMembers || header.AllowRawView)
            {
                if (loadMembers)
                {
                    List<Tupple<string, object>> members = viewer.GetMembers(obj);
                    
                    foreach (Tupple<string, object> member in members)
                        Add(true, parent, member.A + " = ", member.B, false, false);
                    
                    if (header.AllowRawView)
                    {
                        parent = store.AppendValues(parent, "Raw view", GtkProofBox.Box(obj), true);
                        
                        ObjectViewHeader rawHeader = ObjectViewerDirectory.Standard.GetHeader(obj);
                        
                        if (rawHeader.HasMembers)
                            store.AppendValues(parent, "**load on expand placeholder**", null, false);
                    }
                }
                else
                {
                    store.AppendValues(parent, "**load on expand placeholder**", null);
                }
            }
        }
        
        private void OnTreeRowExpanded(object sender, RowExpandedArgs args)
        {
            object obj = store.GetValue(args.Iter, VALUE_OBJECT);
            bool raw = (bool) store.GetValue(args.Iter, VALUE_RAW);
            
            Add(true, args.Iter, null, obj, true, raw);
            
            TreeIter firstChild;
            store.IterChildren(out firstChild, args.Iter);
            store.Remove(ref firstChild);
        }
        
        private void OnTreeRowCollapsed(object sender, RowCollapsedArgs args)
        {
            TreeIter child;
            store.IterChildren(out child, args.Iter);
            while (store.Remove(ref child));
            
            store.AppendValues(args.Iter, "**load on expand placeholder**", null);
        }
        
        private static string GetTitle(object obj)
        {
            return ObjectViewerDirectory.GetDescription(obj);
        }
    }
}
