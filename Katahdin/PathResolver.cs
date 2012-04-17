using System;
using System.IO;

using Katahdin.Collections;

namespace Katahdin
{
    public class PathResolver
    {
        private Stack<string> directories = new Stack<string>();
        
        public PathResolver()
        {
            string path = Environment.GetEnvironmentVariable("KATAHDIN");

            if (path != null)
            {
                foreach (string n in path.Split(Path.PathSeparator))
                {
                    if (n.Length > 0)
                        PushDirectory(n);
                }
            }
        }
        
        public void PushDirectory(string path)
        {
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                
                if (path == "")
                    path = ".";
            }
            
            if (!Directory.Exists(path))
                throw new Exception(path + " does not exist");
            
            directories.Push(path);
        }
        
        public string Resolve(string path)
        {
            if (Path.IsPathRooted(path))
                return path;
            
            for (int n = directories.Count - 1; n >= 0; n--)
            {
                string resolved = Path.Combine(directories[n], path);
                
                if (File.Exists(resolved) || Directory.Exists(resolved))
                    return resolved;
            }
            
            return path;
        }
        
        public void PopDirectory()
        {
            directories.Pop();
        }
        
        public Stack<string> Directories
        {
            get
            {
                return directories;
            }
        }
    }
}
