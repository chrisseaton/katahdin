using System;
using System.Collections.Generic;

using Katahdin.Base;

namespace Katahdin.Grammars
{
    public class Grammar
    {
        private GrammarTrace trace = new GrammarTrace();
        
        private List<Pattern> patterns = new List<Pattern>();
        private Pattern rootPattern;
        
        public void PatternDefined(Pattern pattern)
        {
            patterns.Add(pattern);
            
            trace.PatternDefined(pattern);
        }
        
        public void PatternChanged(params Pattern[] changed)
        {
            List<Pattern> updated = new List<Pattern>();

            foreach (Pattern changedPattern in changed)
            {
                foreach (Pattern pattern in patterns)
                {
                    if (updated.Contains(pattern))
                        continue;

                    AbstractPattern abstractPattern = pattern as AbstractPattern;

                    if (abstractPattern == null)
                        continue;

                    if (abstractPattern.AltPatterns.Contains(changedPattern))
                        abstractPattern.Update(changedPattern);

                    updated.Add(pattern);

                    trace.PatternChanged(abstractPattern);
                }
            }
        }
        
        public RuntimeObject Parse(Lexer lexer, RuntimeState runtimeState)
        {
            runtimeState.Runtime.ParseTrace.Enter(this, null, lexer.FileName);
            
            while (true)
            {
                ParserState state = new ParserState(runtimeState);
                
                ParseTree tree;
                
                if (runtimeState.Runtime.CompileParser)
                    tree = RootPattern.ParseUsingCompiler(lexer, state);
                else
                    tree = RootPattern.Parse(lexer, state);
                
                RuntimeObject obj = (RuntimeObject) tree.Value;
                
                if (!lexer.Finished)
                {
                    string got;
                    
                    if (lexer.MaxPosition < lexer.Text.Length)
                        got = TextEscape.Quote(lexer.Text[lexer.MaxPosition]);
                    else
                        got = "end of source code";
                    
                    throw new ParseException(
                        lexer.SourceAt(lexer.MaxPosition),
                        lexer.ErrorStrings[lexer.MaxPosition],
                        got);
                }
                
                runtimeState.Runtime.ParseTrace.Parsed(lexer.FileName, obj);
                
                if (lexer.ChangingRoots)
                {
                    lexer.Restart();
                    continue;
                }
                
                runtimeState.Runtime.ParseTrace.Yes(this, null);
                
                return obj;
            }
        }
        
        public Pattern RootPattern
        {
            get
            {
                return rootPattern;
            }
            
            set
            {
                rootPattern = value;
            }
        }
        
        public GrammarTrace Trace
        {
            get
            {
                return trace;
            }
        }
    }
}
