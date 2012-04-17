using System;
using System.Collections.Generic;

namespace Katahdin
{
    public class ParseTrace
    {
        public delegate void ProgressChangedHandler(Lexer lexer);
        public event ProgressChangedHandler ProgressChangedEvent;
        
        public delegate void EnterHandler(Source source, string label, object tag, object link);
        public event EnterHandler EnterEvent;
        
        public event Handler LeaveEvent;
        
        public delegate void ParsedHandler(string fileName, RuntimeObject obj);
        public event ParsedHandler ParsedEvent;
        
        private Stack<object> marks = new Stack<object>();
        
        private object nextTag;
        private object nextLink;
        
        public void ProgressChanged(Lexer lexer)
        {
            if (ProgressChangedEvent != null)
                ProgressChangedEvent(lexer);
        }
        
        public void TagNext(object tag)
        {
            nextTag = tag;
        }
        
        public void LinkNext(object tag)
        {
            nextLink = tag;
        }
        
        public void Enter(object mark, Source source, string label)
        {
            marks.Push(mark);
            
            if (EnterEvent != null)
                EnterEvent(source, label, nextTag, nextLink);
            
            nextTag = null;
            nextLink = null;
        }
        
        public void Single(string label)
        {
            Single(null, label);
        }
        
        public void Single(Source source, string label)
        {
            Enter(null, source, label);
            Leave(null);
        }
        
        public void Yes(object mark, Source source)
        {
            Enter(null, source, "yes");
            Leave(null);
            
            Leave(mark);
        }

        public void Yes(object mark, Source source, string message)
        {
            Enter(null, source, "yes: " + message);
            Leave(null);
            
            Leave(mark);
        }
        
        public void No(object mark, Source source)
        {
            Enter(null, source, "no");
            Leave(null);
            
            Leave(mark);
        }

        public void No(object mark, Source source, string message)
        {
            Enter(null, source, "no: " + message);
            Leave(null);
            
            Leave(mark);
        }
        
        public void Leave(object mark)
        {
            if ((marks.Count == 0) || !Object.ReferenceEquals(marks.Pop(), mark))
                throw new Exception("Trace error");
            
            if (LeaveEvent != null)
                LeaveEvent();
        }
        
        public void Parsed(string fileName, RuntimeObject obj)
        {
            if (ParsedEvent != null)
                ParsedEvent(fileName, obj);
        }
    }
}
