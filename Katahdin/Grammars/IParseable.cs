using Katahdin;
using Katahdin.Compiler;

namespace Katahdin.Grammars
{
    public interface IParseable
    {
        ParseTree Parse(Lexer lexer, ParserState state);
        Block Compile(Runtime runtime, StateForCompiler state);
    }
}