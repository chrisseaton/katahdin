using System;
using System.Collections.Generic;
using System.IO;

using Gtk;

namespace Katahdin.Debugger
{
    public class FileView : ScrolledWindow
    {
        private TextView view;
        private TextBuffer buffer;
        
        private TextTag parsedTag;
        private TextTag highlightTag;
        
        public FileView(string fileName)
        {
            BorderWidth = 5;
            ShadowType = ShadowType.In;
        
            view = new TextView();
            view.Editable = false;
            view.CursorVisible = false;
            Add(view);
            
            buffer = view.Buffer;
            buffer.Text = File.ReadAllText(fileName);
            
            parsedTag = new TextTag(null);
            parsedTag.Background = "#ccffcc";
            buffer.TagTable.Add(parsedTag);
            
            highlightTag = new TextTag(null);
            highlightTag.Background = "#ccccff";
            buffer.TagTable.Add(highlightTag);
        }
        
        public void Highlight(Source source, bool highlightUpTo)
        {
            buffer.RemoveAllTags(buffer.StartIter, buffer.EndIter);
            
            if (source != null)
            {
                TextIter startIter = buffer.GetIterAtOffset(source.Position);
                
                if (source.HasExtent)
                {
                    TextIter endIter = buffer.GetIterAtOffset(source.Position + source.Extent);
                    buffer.ApplyTag(highlightTag, startIter, endIter);
                }
                
                if (highlightUpTo)
                    buffer.ApplyTag(parsedTag, buffer.StartIter, startIter);
                
                view.ScrollToIter(startIter, 0, true, 0, 0.5);
            }
        }
    }
}
