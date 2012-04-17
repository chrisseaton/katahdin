using System;
using System.Text;

namespace Katahdin
{
    public abstract class TextEscape
    {
        public static string Quote(string text)
        {
            return "\"" + Escape(text) + "\"";
        }
        
        public static string Unquote(string text)
        {
            if (!((text.StartsWith("\'") && text.EndsWith("\'"))
                || (text.StartsWith("\"") && text.EndsWith("\""))))
                    throw new ArgumentException("Text not quoted");
            
            return Unescape(text.Substring(1, text.Length - 2));
        }
        
        public static string Escape(string text)
        {
            StringBuilder builder = new StringBuilder(text.Length * 2);
            
            foreach (char character in text)
                builder.Append(Escape(character));
            
            return builder.ToString();
        }
        
        public static string Unescape(string text)
        {
            StringBuilder builder = new StringBuilder(text.Length);
            
            for (int n = 0; n < text.Length; n++)
            {
                if (text[n] == '\\')
                {
                    if (n + 1 == text.Length)
                        throw new FormatException("Error in escape sequence");
                    
                    n++;
                    
                    switch (text[n])
                    {
                        case '0': builder.Append("\0"); break;
                        case '\"': builder.Append("\""); break;
                        case '\'': builder.Append("\'"); break;
                        case 'r': builder.Append("\r"); break;
                        case 'n': builder.Append("\n"); break;
                        case 't': builder.Append("\t"); break;
                        case '\\': builder.Append("\\"); break;
                        
                        default:
                            throw new FormatException("Error in escape sequence");
                    }
                }
                else
                {
                    builder.Append(text[n]);
                }
            }
            
            return builder.ToString();
        }
        
        public static string Quote(char character)
        {
            return "\"" + Escape(character) + "\"";
        }

        public static string Escape(char character)
        {
            switch (character)
            {
                case '\0': return "\\0";
                case '\"': return "\\\"";
                case '\r': return "\\r";
                case '\n': return "\\n";
                case '\t': return "\\t";
                case '\\': return "\\\\";
            }
            
            return character.ToString();
        }
    }
}
