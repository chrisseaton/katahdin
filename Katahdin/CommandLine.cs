using System;
using System.Collections.Generic;

namespace Katahdin
{
    public class CommandLine
    {
        private Dictionary<string, string> descriptions = new Dictionary<string, string>();
        private Dictionary<string, object> options = new Dictionary<string, object>();
        private Dictionary<string, string> optionAliases = new Dictionary<string, string>();
        
        private List<string> files = new List<string>();
        private List<string> args = new List<string>();
        
        public CommandLine()
        {
            AddOption("Show this message", false, "-h", "-help", "--help");
            AddOption("Don't load the standard library", false, "-nostd");
        }
        
        public void AddOption(string description, object defaultValue,
            params string[] names)
        {
            descriptions[names[0]] = description;
            options[names[0]] = defaultValue;
            
            for (int n = 1; n < names.Length; n++)
                optionAliases[names[n]] = names[0];
        }
        
        public void Parse(string[] line)
        {
            bool split = false;
            
            foreach (string arg in line)
            {
                if (split)
                {
                    args.Add(arg);
                }
                else if (arg == "-")
                {
                    split = true;
                }
                else if (arg.StartsWith("-"))
                {
                    string name = arg;
                    
                    if (files.Count > 0)
                        throw new Exception("can't have options after you've started specifying files");
                    
                    if (!(options.ContainsKey(name) || optionAliases.ContainsKey(name)))
                        throw new Exception("unexpected option " + name);
                    
                    if (optionAliases.ContainsKey(name))
                        name = optionAliases[name];
                    
                    options[name] = true;
                }
                else
                {
                    files.Add(arg);
                }
            }
        }
        
        public object GetOption(string name)
        {
            if (optionAliases.ContainsKey(name))
                name = optionAliases[name];
            
            return options[name];
        }
        
        public void Help(string name)
        {
			Console.WriteLine(name);
			Console.WriteLine("chris@chrisseaton.com");
			Console.WriteLine();
			Console.WriteLine("Usage:");
			Console.WriteLine("    options files... - arguments...");
			Console.WriteLine();
			Console.WriteLine("Options:");
			
			foreach (string option in options.Keys)
			{
			    string description = descriptions[option];
			    
			    char[] tab = new char[15 - option.Length];
			    
			    for (int n = 0; n < tab.Length; n++)
			        tab[n] = ' ';
			    
			    Console.WriteLine("    " + option + new String(tab) + description);
			}
        }
        
        public List<string> Files
        {
            get
            {
                return files;
            }
        }
        
        public List<string> Args
        {
            get
            {
                return args;
            }
        }
    }
}
