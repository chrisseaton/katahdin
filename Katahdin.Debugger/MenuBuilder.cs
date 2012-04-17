using System;
using System.Collections.Generic;

using Gtk;

namespace Katahdin.Debugger
{
    public class MenuBuilder
    {
        private Stack<MenuShell> stack = new Stack<MenuShell>();
        
        public MenuBar StartMenuBar()
        {
            MenuBar menuBar = new MenuBar();
            
            stack.Push(menuBar);
            
            return menuBar;
        }
        
        public Menu StartMenu()
        {
            return StartMenu(null);
        }
        
        public Menu StartMenu(string title)
        {
            Menu menu = new Menu();
            
            if ((stack.Count > 0) && (title != null))
            {
                MenuShell parent = stack.Peek();
                
                MenuItem item = new MenuItem(title);
                item.Submenu = menu;
                parent.Append(item);
            }
            
            stack.Push(menu);
            
            return menu;
        }
        
        public MenuItem Add(string label, EventHandler activated)
        {
            MenuItem item = new MenuItem(label);
            item.Activated += activated;
            
            if (stack.Count > 0)
            {
                MenuShell parent = stack.Peek();
                parent.Append(item);
            }
            
            return item;
        }
        
        public CheckMenuItem AddCheck(string label, EventHandler toggled)
        {
            CheckMenuItem item = new CheckMenuItem(label);
            item.Toggled += toggled;
            
            if (stack.Count > 0)
            {
                MenuShell parent = stack.Peek();
                parent.Append(item);
            }
            
            return item;
        }
        
        public void Separate()
        {
            MenuShell parent = stack.Peek();
            parent.Append(new SeparatorMenuItem());
        }
        
        public void End()
        {
            stack.Pop();
        }
    }
}
