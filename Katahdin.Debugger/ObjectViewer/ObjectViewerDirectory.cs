using System;
using System.Collections.Generic;

namespace Katahdin.Debugger.ObjectViewer
{
    public class ObjectViewerDirectory
    {
        private static StandardObjectViewer standard = new StandardObjectViewer();
        private static List<IObjectViewer> viewers = new List<IObjectViewer>();
        
        static ObjectViewerDirectory()
        {
            Prepend(new SimpleObjectViewer());
            Prepend(new StringObjectViewer());
            Prepend(new ArrayObjectViewer());
            Prepend(new ListObjectViewer());
            Prepend(new DictionaryObjectViewer());
        }
        
        public static void Prepend(IObjectViewer viewer)
        {
            viewers.Add(viewer);
        }
        
        public static IObjectViewer Choose(object obj)
        {
            if (obj == null)
                return standard;
            
            for (int n = viewers.Count - 1; n >= 0; n--)
            {
                if (viewers[n].Views(obj))
                    return viewers[n];
            }
            
            return standard;
        }
        
        public static string GetDescription(object obj)
        {
            IObjectViewer viewer = Choose(obj);
            return viewer.GetHeader(obj).Description;
        }
        
        public static StandardObjectViewer Standard
        {
            get
            {
                return standard;
            }
        }
    }
}
