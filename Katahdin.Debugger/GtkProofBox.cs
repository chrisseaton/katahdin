using System;

namespace Katahdin.Debugger
{
    public class GtkProofBox
    {
        private object contents;
        
        public GtkProofBox(object contents)
        {
            this.contents = contents;
        }
        
        public object Contents
        {
            get
            {
                return contents;
            }
            
            set
            {
                contents = value;
            }
        }
        
        public static object Box(object obj)
        {
            if ((obj is string) || (obj is ValueType))
                return new GtkProofBox(obj);
            else
                return obj;
        }
        
        public static object Unbox(object obj)
        {
            if (obj is GtkProofBox)
                return ((GtkProofBox) obj).Contents;
            else
                return obj;
        }
    }
}