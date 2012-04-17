using System;
using System.Collections.Generic;


namespace Katahdin.Grammars
{
    public class BootstrapParser
    {
        private string text;
        private int position;
        
        private Pattern[] patterns;
        
        public BootstrapParser(string text, Pattern[] patterns)
        {
            this.text = text;
            this.patterns = patterns;
        }
        
        public static ParseGraphNode Parse(string text, Pattern[] patterns)
        {
            BootstrapParser lexer = new BootstrapParser(text, patterns);
            return lexer.ParseNode();
        }
        
        private ParseGraphNode ParseNode()
        {
            string token = Read();
            
            if ((token == "s") || (token == "a"))
            {
                Read("(");
                
                List<ParseGraphNode> nodes = new List<ParseGraphNode>();
                
                while (Peek() != ")")
                    nodes.Add(ParseNode());
                
                Read(")");
                
                if (token == "s")
                    return new SeqNode(null, nodes);
                else if (token == "a")
                    return new AltNode(null, nodes, false);
            }
            else if ((token == "?") || (token == "*") || (token == "+"))
            {
                Read("(");
                ParseGraphNode body = ParseNode();
                Read(")");

                return new RepNode(null, body, Reps.ForName(token[0]));
            }
            else if (token == "&")
            {
                Read("(");
                ParseGraphNode body = ParseNode();
                Read(")");

                return new AndNode(null, body);
            }
            else if (token == "!")
            {
                Read("(");
                ParseGraphNode body = ParseNode();
                Read(")");

                return new NotNode(null, body);
            }
            else if (token == "r")
            {
                Read("(");
                
                char min = TextEscape.Unquote(Read())[0];
                char max = TextEscape.Unquote(Read())[0];
                
                Read(")");
                
                return new CharNode(null, new CharRange(min, max));
            }
            else if (token == "l")
            {
                Read("(");

                string label = Read();
                ParseGraphNode body = ParseNode();

                Read(")");

                return new LabelNode(null, label, body);
            }
            else if (token == "t")
            {
                Read("(");

                ParseGraphNode body = ParseNode();

                Read(")");

                return new TokenNode(null, body);
            }
            else if (token == "o")
            {
                Read("(");
                
                Dictionary<string, object> options = new Dictionary<string, object>();
                
                while (Peek() == "(")
                {
                    Read();
                    
                    string optionName = Read();
                    object optionValue = ReadValue();
                    
                    options[optionName] = optionValue;
                    
                    Read(")");
                }
                
                ParseGraphNode body = ParseNode();

                Read(")");
                
                OptionsNode optionsNode = new OptionsNode(null, body);
                
                foreach (string key in options.Keys)
                {
                    switch (key)
                    {
                        case "buildTextNodes":
                            optionsNode.BuildTextNodes.Value = (bool) options[key];
                            break;
                        
                        case "dropPrecedence":
                            optionsNode.DropPrecedence.Value = (bool) options[key];
                            break;
                        
                        case "recursive":
                        {
                            if ((bool) options[key])
                                optionsNode.RecursionBehaviour.Value = RecursionBehaviour.Recursive;
                            else
                                optionsNode.RecursionBehaviour.Value = RecursionBehaviour.None;
                        } break;

                        case "leftRecursive":
                            optionsNode.RecursionBehaviour.Value = RecursionBehaviour.LeftRecursive;
                            break;

                        case "rightRecursive":
                            optionsNode.RecursionBehaviour.Value = RecursionBehaviour.RightRecursive;
                            break;
                        
                        case "whitespace":
                        {
                            object value = options[key];
                            
                            if (value == null)
                                optionsNode.Whitespace.Value = null;
                            else
                                optionsNode.Whitespace.Value = Pattern.PatternForType((Type) value);
                        } break;

                        case "exclude":
                            optionsNode.Exclude.Value = Pattern.PatternForType((Type) options[key]);
                            break;
                    }
                }

                return optionsNode;
            }
            else if (token == "any")
            {
                return new AnyNode(null);
            }
            else if ((token[0] >= '0') && (token[0] <= '9'))
            {
                return new PatternNode(null, patterns[Convert.ToInt32(token)], false);
            }
            else if (token[0] == '\'')
            {
                string text = TextEscape.Unquote(token);
                
                if (text.Length == 1)
                    return new CharNode(null, text[0]);
                else
                    return new TextNode(null, text);
            }
            
            throw new Exception(token);
        }
        
        private object ReadValue()
        {
            string x = Read();
            
            if ((x[0] >= '0') && (x[0] <= '9'))
                return patterns[Convert.ToInt32(x)].Type;
            
            switch (x)
            {
                case "true": return true;
                case "false": return false;
                case "null": return null;
            }
            
            throw new Exception();
        }
        
        private string Peek()
        {
            int start = position;
            string token = Read();
            position = start;
            
            return token;
        }
        
        private void Read(string expected)
        {
            string got = Read();
            
            if (got != expected)
                throw new Exception("Expecting " + expected + " but got " + got);
        }
        
        private string Read()
        {
            // Skip whitespace
            
            while (text[position] == ' ')
                position++;
            
            // Single character tokens
            
            switch (text[position])
            {
                case '(':
                case ')':
                case '?':
                case '*':
                case '+':
                case '&':
                case '!':
                {
                    position++;
                    return text.Substring(position - 1, 1);
                }
            }
            
            // Strings
            
            int start = position;
            
            if (text[position] == '\'')
            {
                position++;
                
                while (text[position] != '\'')
                {
                    if (text[position] == '\\')
                        position++;
                    
                    position++;
                }
                
                position++;
                
                return text.Substring(start, position - start);
            }
            
            // Numbers
            
            if ((text[position] >= '0') && (text[position] <= '9'))
            {
                while ((position < text.Length)
                    && (text[position] >= '0') && (text[position] <= '9'))
                        position++;
                
                return text.Substring(start, position - start);
            }
            
            // Words
            
            while ((position < text.Length)
                && (((text[position] >= 'a') && (text[position] <= 'z'))
                 || ((text[position] >= 'A') && (text[position] <= 'Z'))))
                    position++;
            
            if (position == start)
                Console.WriteLine(text.Substring(start, 5));
            
            return text.Substring(start, position - start);
        }
    }
}
