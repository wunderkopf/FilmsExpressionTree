using System.Collections.Generic;
using System.IO;

namespace FilmsExpressionTree
{
    public class Scanner
    {
        private class Token : IToken
        {
            static private List<char> operators = new List<char>()
            {
                '<',
                '>',
                '='
            };

            static private List<string> functions = new List<string>()
            {
                "contains"
            };

            public TokenType Type { get; set; }

            public string Value { get; set; }

            public Token(StringReader reader)
            {
                int c = -1;
                while ((c = reader.Read()) == ' ') ;
                if (c == -1)
                {
                    Type = TokenType.End;
                    Value = string.Empty;
                }
                else
                {
                    char ch = (char)c;
                    string tokenValue = new string(new char[] { ch });
                    if (operators.Contains(ch))
                    {
                        Type = TokenType.Operator;
                        if (operators.Contains((char)reader.Peek()))
                        {
                            ch = (char)reader.Read();
                            tokenValue += ch;
                        }
                        Value = tokenValue;
                    }
                    else
                    {
                        while (reader.Peek() != -1 && !operators.Contains((char)reader.Peek()))
                        {
                            ch = (char)reader.Read();
                            if (ch == ' ')
                                break;
                            tokenValue += ch;
                        }

                        if (tokenValue.StartsWith('.'))
                            Type = TokenType.Property;
                        else if (functions.Contains(tokenValue))
                            Type = TokenType.Function;
                        else
                            Type = TokenType.Literal;
                        Value = tokenValue;
                    }
                }
            }
        }
        private StringReader reader;
        private string expression;

        public Scanner(string expression)
        {
            this.Expression = expression.Trim();
            reader = new StringReader(Expression);
        }

        public string Expression
        {
            get
            {
                return this.expression;
            }
            set
            {
                this.expression = value;
                reader = new StringReader(this.expression);
            }
        }

        public IToken Next()
        {
            var token = new Token(reader);
            return token;
        }
    }
}