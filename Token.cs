using System.Collections.Generic;
using System.IO;

namespace FilmsExpressionTree
{
    public enum TokenType
    {
        Operator,
        Literal,
        Function,
        Property,
        Operation,
        End
    }
    public interface IToken
    {
        TokenType Type { get; set; }

        string Value { get; set; }
    }
}