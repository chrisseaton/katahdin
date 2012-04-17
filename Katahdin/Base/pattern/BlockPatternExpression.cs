using System;
using System.Collections.Generic;

using Katahdin.Grammars;
using Katahdin.CodeTree;

namespace Katahdin.Base
{
    public class BlockPatternExpression : PatternExpression
    {
        public new static Pattern pattern;
        
        public List<object> options;
        public PatternExpression body;
        
        public BlockPatternExpression(Source source, object options, object body)
            : base(source)
        {
            this.options = (List<object>) options;
            this.body = (PatternExpression) body;
        }
        
        public new static void SetUp(Module module, Grammar grammar)
        {
            if (pattern == null)
            {
                string expression = "s('{' l(options *(0)) o((dropPrecedence true) l(body 1)) '}')";
                Pattern[] patterns = {Option.pattern, PatternExpression.pattern};
            
                ParseGraphNode parseGraph = BootstrapParser.Parse(expression, patterns);
        
                pattern = new ConcretePattern(null, "BlockPatternExpression", parseGraph);
                pattern.SetType(typeof(BlockPatternExpression));
            
                PatternExpression.pattern.AddAltPattern(pattern);
            }
            
            module.SetName("BlockPatternExpression", typeof(BlockPatternExpression));
            grammar.PatternDefined(pattern);
        }
        
        public override ParseGraphNode BuildParseGraph(RuntimeState state)
        {
            ParseGraphNode parseGraph = body.BuildParseGraph(state);
            
            if (options.Count == 0)
                return parseGraph;
                
            OptionsNode optionsNode = new OptionsNode(Source, parseGraph);
            
            foreach (Option option in options)
            {
                string optionKey = option.optionKey.name;
                object optionValue;
                
                if (option.optionValue == null)
                    optionValue = true;
                else
                    optionValue = option.optionValue.Get(state);
                
                switch (optionKey)
                {
                    case "buildTextNodes":
                        optionsNode.BuildTextNodes.Value = ConvertNode.ToBool(optionValue);
                        break;
                    
                    case "dropPrecedence":
                        optionsNode.DropPrecedence.Value = ConvertNode.ToBool(optionValue);
                        break;
                    
                    case "recursive":
                    {
                        if (ConvertNode.ToBool(optionValue))
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
                        optionsNode.Whitespace.Value = Pattern.PatternForType((Type) optionValue);
                        break;
                    
                    case "exclude":
                        optionsNode.Exclude.Value = Pattern.PatternForType((Type) optionValue);
                        break;
                }
            }
            
            return optionsNode;
        }
    }
}
