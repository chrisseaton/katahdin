using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

using Katahdin.Collections;
using Katahdin.Grammars;

namespace Katahdin
{
    public class Lexer
    {
        private ParseTrace parseTrace;
        
        private string fileName;
        private string text;
        
        private int position;
        private int length;
        
        private Pattern whitespace;
        private bool handleWhitespace = true;
        
        private int maxPosition;
        
        private bool changingRoots;
        
        // TODO
        public Set<string>[] errorStrings;
        
        public Lexer(ParseTrace parseTrace, string fileName)
        {
            this.parseTrace = parseTrace;
            this.fileName = fileName;
            
            text = File.ReadAllText(fileName);
            
            errorStrings = new Set<string>[text.Length + 1];
            
            for (int n = 0; n < errorStrings.Length; n++)
                errorStrings[n] = new Set<string>();
            
            Restart();
        }
        
        public void RootChanged()
        {
            changingRoots = true;
            
            /*
                Make the current position look like the end of the file so
                that the lexer will finish off the root pattern.
            */
            
            length = position;
        }
        
        public void Restart()
        {
            // Also called by constructor
            
            changingRoots = false;
            length = text.Length;
            whitespace = Base.Whitespace.pattern;
        }

        public void Whitespace(RuntimeState runtimeState)
        {
            //while (Peek().ToString().Trim().Length == 0)
            //    Read();
            
            if (!handleWhitespace)
                return;
            
            if (position >= length)
                return;
            
            if (whitespace == null)
                return;
            
            ParserState state = new ParserState(runtimeState);
        
            bool previousHandleWhitespace = handleWhitespace;
            handleWhitespace = false;
        
            whitespace.Parse(this, state);
            
            handleWhitespace = previousHandleWhitespace;
        }
        
        public void QuickWhitespace()
        {
            int start = position;
            
            while (position < text.Length)
            {
                char character = text[position];
                
                if ((character == '/') && (position + 1 < text.Length))
                {
                    if (text[position + 1] == '/')
                    {
                        position += 2;
                        
                        while (position < text.Length)
                        {
                            character = text[position];
                            
                            if ((character == '\r') || (character == '\n'))
                                break;
                            
                            position++;
                        }
                        
                        continue;
                    }
                    else if (text[position + 1] == '*')
                    {
                        position += 2;
                        
                        int depth = 1;
                        
                        while ((position < text.Length) && (depth > 0))
                        {
                            if (position + 1 < text.Length)
                            {
                                if ((text[position] == '*')
                                    && (text[position + 1] == '/'))
                                {
                                    position += 2;
                                    depth--;
                                    
                                    if (depth < 0)
                                        break;
                                    
                                    continue;
                                }
                                else if ((text[position] == '/')
                                    && (text[position + 1] == '*'))
                                {
                                    position += 2;
                                    depth++;
                                    continue;
                                }
                            }
                            
                            position++;
                        }
                        
                        if (depth < 0)
                        {
                            position = start;
                            return;
                        }
                        
                        continue;
                    }
                }
                
                if ((character != ' ')
                    && (character != '\t')
                    && (character != '\r')
                    && (character != '\n'))
                        break;
                
                position++;
            }
        }
        
        public void QuickDefaultWhitespace()
        {
            while (position < text.Length)
            {
                char character = text[position];
                
                if ((character != ' ')
                    && (character != '\t')
                    && (character != '\r')
                    && (character != '\n'))
                        break;
                
                position++;
            }
        }
        
        public char Peek()
        {
            return Peek(0);
        }
        
        public char Peek(int ahead)
        {
            int n = position + ahead;
            
            if (n < length)
                return text[n];
            else
                return '\0';
        }
        
        public char Read()
        {
            if (position < length)
            {
                char character = text[position];
                Skip(1);
                return character;
            }
            else
                return '\0';
        }
        
        public void Skip(int length)
        {
            position += length;
            
            if (position > maxPosition)
            {
                maxPosition = position;
                parseTrace.ProgressChanged(this);
            }
        }
        
        public Source CurrentSource()
        {
            return SourceAt(position);
        }
        
        public Source SourceFrom(int start)
        {
            return new Source(fileName, LineAtPosition(start), start,
                position - start);
        }
        
        public Source SourceAt(int position)
        {
            return new Source(fileName, LineAtPosition(position), position);
        }
        
        private int LineAtPosition(int position)
        {
            int line = 1;
            
            for (int n = 0; n < position; n++)
            {
                if (text[n] == '\r')
                {
                    if (text[n + 1] == '\n')
                        n++;
                    
                    line++;
                }
                else if (text[n] == '\n')
                {
                    line++;
                }
            }
            
            return line;
        }

        public bool Finished
        {
            get
            {
                return position >= length;
            }
        }
        
        public string FileName
        {
            get
            {
                return fileName;
            }
        }
        
        public string Text
        {
            get
            {
                return text;
            }
        }
        
        public int Position
        {
            get
            {
                if (position == -1)
                    throw new Exception();
                
                return position;
            }
            
            set
            {
                if (value == -1)
                    throw new Exception();
                
                position = value;
            }
        }
        
        public int MaxPosition
        {
            get
            {
                return maxPosition;
            }
        }
        
        public int Length
        {
            get
            {
                return length;
            }
            
            set
            {
                length = value;
                Debug.Assert(length <= text.Length);
            }
        }
        
        public Pattern WhitespacePattern
        {
            get
            {
                return whitespace;
            }
            
            set
            {
                whitespace = value;
            }
        }
        
        public double Progress
        {
            get
            {
                return maxPosition / (double) text.Length;
            }
        }
        
        public Set<string>[] ErrorStrings
        {
            get
            {
                return errorStrings;
            }
        }
        
        public bool ChangingRoots
        {
            get
            {
                return changingRoots;
            }
            
            set
            {
                changingRoots = value;
            }
        }
    }
}
