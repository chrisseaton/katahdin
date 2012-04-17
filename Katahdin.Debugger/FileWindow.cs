using System;

using Gtk;

namespace Katahdin.Debugger
{
    public class FileWindow : Window
    {
        private FileView fileView;
        
        public FileWindow(string fileName)
            : base(fileName)
        {
            fileView = new FileView(fileName);
            Add(fileView);
        }
        
        protected override bool OnDeleteEvent(Gdk.Event args)
        {
            Hide();

            return true;
        }
        
        public FileView FileView
        {
            get
            {
                return fileView;
            }
        }
    }
}
