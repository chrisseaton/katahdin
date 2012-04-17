using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

using Katahdin.Collections;
using Katahdin.Grammars;
using Katahdin.CodeTree;
using Katahdin.Compiler;
using Katahdin.Base;

using Katahdin.Grammars.Precedences;

namespace Katahdin
{
    public class Runtime
    {
        private Module rootModule;
        
        private Grammar grammar = new Grammar();
        private CompilerModule compilerModule = new CompilerModule();
        
        private ParseTrace parseTrace = new ParseTrace();
        
        private bool hasBeenSetUp;
        
        private PathResolver pathResolver = new PathResolver();
        
        private Lexer currentLexer;
        
        private Set<string> imported = new Set<string>();
        
        private bool compileParser;
        
        private bool traceParser;
        private bool traceParseTrees;
        private bool traceGrammar;
        private bool verbose;
        
        public Runtime(bool compileParser, bool traceParser,
            bool traceParseTrees, bool traceGrammar, bool verbose)
        {
            this.compileParser = compileParser;
            this.traceParser = traceParser;
            this.traceParseTrees = traceParseTrees;
            this.traceGrammar = traceGrammar;
            this.verbose = verbose;
        }
        
        public void SetUp(List<string> args)
        {
            if (verbose)
            {
                Console.WriteLine("import path:");
                
                foreach (string directory in pathResolver.Directories)
                    Console.WriteLine("  " + directory);
            }
            
            AppDomain domain = Thread.GetDomain();
            
		    rootModule = new Module(null, null);
		    
		    foreach (Assembly assembly in domain.GetAssemblies())
		        AssemblyLoaded(assembly);
		    
		    domain.AssemblyLoad += OnDomainAssemblyLoad;
		    
		    rootModule.SetName("null", null);
		    rootModule.SetName("true", true);
    		rootModule.SetName("false", false);
    		rootModule.SetName("args", args);
    		
	        DefaultWhitespace.SetUp(rootModule, grammar);
	        LineComment.SetUp(rootModule, grammar);
	        BlockComment.SetUp(rootModule, grammar);
	        Whitespace.SetUp(rootModule, grammar);
		    Name.SetUp(rootModule, grammar);
		    Name.SetUp(rootModule, grammar);
		    Number.SetUp(rootModule, grammar);
		    Base.String.SetUp(rootModule, grammar);
		    
		    Expression.SetUp(rootModule, grammar);
		    ValueExpression.SetUp(rootModule, grammar);
		    NameExpression.SetUp(rootModule, grammar);
		    ParenExpression.SetUp(rootModule, grammar);
		    MemberExpression.SetUp(rootModule, grammar);
		    CallExpression.SetUp(rootModule, grammar);
		    CallInParentScopeExpression.SetUp(rootModule, grammar);
		    NewExpression.SetUp(rootModule, grammar);
		    TypeExpression.SetUp(rootModule, grammar);
		    IsExpression.SetUp(rootModule, grammar);
		    AsExpression.SetUp(rootModule, grammar);
		    UnaryExpression.SetUp(rootModule, grammar);
		    NotExpression.SetUp(rootModule, grammar);
		    MultiplicativeExpression.SetUp(rootModule, grammar);
		    MultiplyExpression.SetUp(rootModule, grammar);
		    DivideExpression.SetUp(rootModule, grammar);
		    AdditiveExpression.SetUp(rootModule, grammar);
		    AddExpression.SetUp(rootModule, grammar);
		    SubtractExpression.SetUp(rootModule, grammar);
		    ComparisonExpression.SetUp(rootModule, grammar);
		    LessExpression.SetUp(rootModule, grammar);
		    LessOrEqualExpression.SetUp(rootModule, grammar);
		    EqualityExpression.SetUp(rootModule, grammar);
		    InequalityExpression.SetUp(rootModule, grammar);
		    GreaterExpression.SetUp(rootModule, grammar);
		    GreaterOrEqualExpression.SetUp(rootModule, grammar);
		    JunctionExpression.SetUp(rootModule, grammar);
		    AndExpression.SetUp(rootModule, grammar);
		    AssignmentExpression.SetUp(rootModule, grammar);
		    AssignExpression.SetUp(rootModule, grammar);
		    
		    /*
		        NameExpression = ValueExpression
    		    ParenExpression = ValueExpression
    		    
    		    CallExpression < ValueExpression
    		    CallInParentScopeExpression = CallExpression
    		    MemberExpression = CallExpression
    		    
    		    NewExpression < CallExpression
    		    TypeExpression < NewExpression
    		    UnaryExpression < TypeExpression
    		    MultiplicativeExpression < UnaryExpression
    		    AdditiveExpression < MultiplicativeExpression
    		    ComparisonExpression < AdditiveExpression
    		    JunctionExpression < ComparisonExpression
    		    AssignmentExpression < JunctionExpression
		    */
		    
		    Precedence.SetPrecedence(NameExpression.pattern.Precedence,
		        ValueExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(ParenExpression.pattern.Precedence,
		        ValueExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(CallExpression.pattern.Precedence,
		        ValueExpression.pattern.Precedence, Relation.Lower);

		    Precedence.SetPrecedence(CallInParentScopeExpression.pattern.Precedence,
		        CallExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(MemberExpression.pattern.Precedence,
		        CallExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(NewExpression.pattern.Precedence,
		        CallExpression.pattern.Precedence, Relation.Lower);
		    
    		Precedence.SetPrecedence(TypeExpression.pattern.Precedence,
    		    NewExpression.pattern.Precedence, Relation.Lower);
		    
    		Precedence.SetPrecedence(UnaryExpression.pattern.Precedence,
    		    TypeExpression.pattern.Precedence, Relation.Lower);
		    
    		Precedence.SetPrecedence(MultiplicativeExpression.pattern.Precedence,
    		    UnaryExpression.pattern.Precedence, Relation.Lower);
		    
    		Precedence.SetPrecedence(AdditiveExpression.pattern.Precedence,
    		    MultiplicativeExpression.pattern.Precedence, Relation.Lower);
		    
    		Precedence.SetPrecedence(ComparisonExpression.pattern.Precedence,
    		    AdditiveExpression.pattern.Precedence, Relation.Lower);
		    
    		Precedence.SetPrecedence(JunctionExpression.pattern.Precedence,
    		    ComparisonExpression.pattern.Precedence, Relation.Lower);
		    
    		Precedence.SetPrecedence(AssignmentExpression.pattern.Precedence,
    		    JunctionExpression.pattern.Precedence, Relation.Lower);

		    Grammar.PatternChanged(ValueExpression.pattern,
		                           NameExpression.pattern,
		                           ParenExpression.pattern,
		                           MemberExpression.pattern,
		                           CallExpression.pattern,
		                           NewExpression.pattern,
		                           TypeExpression.pattern,
		                           UnaryExpression.pattern,
		                           MultiplicativeExpression.pattern,
		                           AdditiveExpression.pattern,
		                           ComparisonExpression.pattern,
   		                           JunctionExpression.pattern,
		                           AssignmentExpression.pattern);
		    
		    PatternExpression.SetUp(rootModule, grammar);
		    ReferencePatternExpression.SetUp(rootModule, grammar);
		    AnyPatternExpression.SetUp(rootModule, grammar);
		    TextPatternExpression.SetUp(rootModule, grammar);
		    Option.SetUp(rootModule, grammar);
		    BlockPatternExpression.SetUp(rootModule, grammar);
		    ParenPatternExpression.SetUp(rootModule, grammar);
		    TokenPatternExpression.SetUp(rootModule, grammar);
		    RangePatternExpression.SetUp(rootModule, grammar);
		    RepeatPatternExpression.SetUp(rootModule, grammar);
		    AndPatternExpression.SetUp(rootModule, grammar);
		    NotPatternExpression.SetUp(rootModule, grammar);
		    LabelPatternExpression.SetUp(rootModule, grammar);
		    SequencePatternExpression.SetUp(rootModule, grammar);
		    AltPatternExpression.SetUp(rootModule, grammar);
        
		    /*
		        EndPatternExpression = ReferencePatternExpression
    		    AnyPatternExpression = ReferencePatternExpression
        		TextPatternExpression = ReferencePatternExpression
        		BlockPatternExpression = ReferencePatternExpression
        		ParenPatternExpression = ReferencePatternExpression
        		TokenPatternExpression = ReferencePatternExpression
        		
        		RangePatternExpression < ReferencePatternExpression
        		
        		AndPatternExpression < RangePatternExpression
        		NotPatternExpression = AndPatternExpression
        		RepeatPatternExpression = AndPatternExpression
        		
        		LabelPatternExpression < AndPatternExpression
        		SequencePatternExpression < LabelPatternExpression
        		AltPatternExpression < SequencePatternExpression
		    */

		    Precedence.SetPrecedence(AnyPatternExpression.pattern.Precedence,
		        ReferencePatternExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(TextPatternExpression.pattern.Precedence,
		        ReferencePatternExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(BlockPatternExpression.pattern.Precedence,
		        ReferencePatternExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(ParenPatternExpression.pattern.Precedence,
		        ReferencePatternExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(TokenPatternExpression.pattern.Precedence,
		        ReferencePatternExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(RangePatternExpression.pattern.Precedence,
		        ReferencePatternExpression.pattern.Precedence, Relation.Lower);

		    Precedence.SetPrecedence(AndPatternExpression.pattern.Precedence,
		        RangePatternExpression.pattern.Precedence, Relation.Lower);

		    Precedence.SetPrecedence(NotPatternExpression.pattern.Precedence,
		        AndPatternExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(RepeatPatternExpression.pattern.Precedence,
		        AndPatternExpression.pattern.Precedence, Relation.Equal);

		    Precedence.SetPrecedence(LabelPatternExpression.pattern.Precedence,
		        AndPatternExpression.pattern.Precedence, Relation.Lower);

		    Precedence.SetPrecedence(SequencePatternExpression.pattern.Precedence,
		        LabelPatternExpression.pattern.Precedence, Relation.Lower);

		    Precedence.SetPrecedence(AltPatternExpression.pattern.Precedence,
		        SequencePatternExpression.pattern.Precedence, Relation.Lower);

		    Grammar.PatternChanged(ReferencePatternExpression.pattern,
		                           AnyPatternExpression.pattern,
		                           TextPatternExpression.pattern,
		                           BlockPatternExpression.pattern,
		                           ParenPatternExpression.pattern,
		                           TokenPatternExpression.pattern,
		                           RangePatternExpression.pattern,
		                           RepeatPatternExpression.pattern,
   		                           AndPatternExpression.pattern,
   		                           NotPatternExpression.pattern,
		                           LabelPatternExpression.pattern,
		                           SequencePatternExpression.pattern,
   		                           AltPatternExpression.pattern);
            
		    Statement.SetUp(rootModule, grammar);
            ExpressionStatement.SetUp(rootModule, grammar);
		    CompoundStatement.SetUp(rootModule, grammar);
		    PrintStatement.SetUp(rootModule, grammar);
		    IfStatement.SetUp(rootModule, grammar);
		    WhileStatement.SetUp(rootModule, grammar);
		    ReturnStatement.SetUp(rootModule, grammar);
		    ThrowStatement.SetUp(rootModule, grammar);
		    TryStatement.SetUp(rootModule, grammar);
		    ModuleStatement.SetUp(rootModule, grammar);
		    FunctionStatement.SetUp(rootModule, grammar);
		    Member.SetUp(rootModule, grammar);
		    PatternMember.SetUp(rootModule, grammar);
		    FieldMember.SetUp(rootModule, grammar);
		    ConstructorMember.SetUp(rootModule, grammar);
		    MethodMember.SetUp(rootModule, grammar);
		    ClassStatement.SetUp(rootModule, grammar);
		    SetPrecedenceStatement.SetUp(rootModule, grammar);
		    UsingStatement.SetUp(rootModule, grammar);
		    ImportStatement.SetUp(rootModule, grammar);
		    TopLevelStatement.SetUp(rootModule, grammar);
		    Program.SetUp(rootModule, grammar);
            
		    Grammar.PatternChanged(Member.pattern, Statement.pattern);
            
            grammar.RootPattern = Program.pattern;
            
            hasBeenSetUp = true;
        }
        
        private void OnDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            AssemblyLoaded(args.LoadedAssembly);
        }
        
        private void AssemblyLoaded(Assembly assembly)
        {
	        if (assembly is AssemblyBuilder)
	            return;
	        
	        foreach (Type type in assembly.GetExportedTypes())
	        {
                Module module = rootModule;
                
                if (type.Namespace != null)
                {
	                foreach (string part in
	                    type.Namespace.Split(new char[]{Type.Delimiter}))
	                {
	                    if (module.IsDefined(part))
	                    {
                            module = (Module) module.GetName(part);
	                    }
	                    else
	                    {
	                        Module child = new Module(module, part);
	                        module.SetName(child.Name, child);
	                        module = child;
	                    }
	                }
                }
                
                module.SetName(type.Name, type);
	        }
        }
        
        public void ImportStandard()
        {
            Import("standard.kat");
        }
        
        public void Import(string fileName)
        {
            if (!hasBeenSetUp)
                throw new Exception();
            
            fileName = pathResolver.Resolve(fileName);
            
            string absoluteFileName = Path.GetFullPath(fileName);
            
            if (imported.Contains(absoluteFileName))
                return;
            
            if (verbose)
                Console.WriteLine("importing " + fileName);
            
            imported.Add(absoluteFileName);
            
            if (Path.GetExtension(fileName).ToLower() == ".dll")
            {
                Assembly.LoadFrom(fileName);
            }
            else
            {
                pathResolver.PushDirectory(fileName);
            
                Lexer lexer = new Lexer(parseTrace, fileName);
                RuntimeState runtimeState = new RuntimeState(this, rootModule);
            
                currentLexer = lexer;
                
                Pattern oldRoot = grammar.RootPattern;
                
                try
                {
                    string extension = Path.GetExtension(fileName).ToLower();
                    
                    if ((extension != "") && (extension != ".kat"))
                    {
                        IDictionary languages = (IDictionary) rootModule.GetName("languages");
                        object callable = languages[extension];
                        Type rootType = (Type) CallNode.Call(runtimeState, callable, null);
                            
                        grammar.RootPattern = Pattern.PatternForType(rootType);
                    }
                    
                    grammar.Parse(lexer, runtimeState);
                }
                catch (Exception exception)
                {
                    if (runtimeState.RunningSource != null)
                        Console.WriteLine(runtimeState.RunningSource);
                    
                    throw exception;
                }
                finally
                {
                    grammar.RootPattern = oldRoot;
                }
                
                pathResolver.PopDirectory();
            }
        }
        
        public PathResolver PathResolver
        {
            get
            {
                return pathResolver;
            }
        }

        public Module RootModule
        {
            get
            {
                return rootModule;
            }
        }

        public Grammar Grammar
        {
            get
            {
                return grammar;
            }
        }
        
        public CompilerModule CompilerModule
        {
            get
            {
                return compilerModule;
            }
        }
        
        public bool CompileParser
        {
            get
            {
                return compileParser;
            }
        }
        
        public ParseTrace ParseTrace
        {
            get
            {
                return parseTrace;
            }
        }
        
        public Lexer CurrentLexer
        {
            get
            {
                return currentLexer;
            }
        }
        
        public bool TraceParser
        {
            get
            {
                return traceParser;
            }
        }
        
        public bool TraceParseTrees
        {
            get
            {
                return traceParseTrees;
            }
        }
        
        public bool TraceGrammar
        {
            get
            {
                return traceGrammar;
            }
        }
        
        public bool Verbose
        {
            get
            {
                return verbose;
            }
        }
    }
}
