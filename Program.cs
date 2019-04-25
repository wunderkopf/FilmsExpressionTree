using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FilmsExpressionTree.Models;

namespace FilmsExpressionTree
{
    class Program
    {
        public static IEnumerable<Film> SeedData()
        {
            Genre drama = new Genre
            {
                Title = "drama"
            };

            Genre action = new Genre
            {
                Title = "action"
            };

            Genre comedy = new Genre
            {
                Title = "comedy"
            };

            Genre fantasy = new Genre
            {
                Title = "fantasy"
            };

            Genre sf = new Genre
            {
                Title = "sci-fi"
            };

            Genre thriller = new Genre
            {
                Title = "thriller"
            };

            Genre horror = new Genre
            {
                Title = "horror"
            };

            Film f1 = new Film
            {
                Title = "Fight Club",
                Year = 1999
            };

            Film f2 = new Film
            {
                Title = "The Dark Knight",
                Year = 2008
            };

            Film f3 = new Film
            {
                Title = "Star Wars: Episode V - The Empire Strikes Back",
                Year = 1980
            };

            Film f4 = new Film
            {
                Title = "Alien",
                Year = 1979
            };

            Film f5 = new Film
            {
                Title = "Trainspotting",
                Year = 1996
            };

            // ----
            f1.AddGenre(drama);
            f1.AddGenre(action);
            f1.AddGenre(thriller);
            yield return f1;

            f2.AddGenre(drama);
            f2.AddGenre(action);
            f2.AddGenre(fantasy);
            yield return f2;

            f3.AddGenre(drama);
            f3.AddGenre(action);
            f3.AddGenre(fantasy);
            f3.AddGenre(sf);
            yield return f3;

            f4.AddGenre(drama);
            f4.AddGenre(action);
            f4.AddGenre(horror);
            f4.AddGenre(sf);
            f4.AddGenre(thriller);
            yield return f4;

            f5.AddGenre(drama);
            f5.AddGenre(action);
            f5.AddGenre(comedy);
            yield return f5;
        }

        static IEnumerable<IToken> Tokenize(string query)
        {
            Scanner scanner = new Scanner(query);
            IToken token = scanner.Next();
            yield return token;
            while (token.Type != TokenType.End)
            {
                token = scanner.Next();
                yield return token;
            }
        }

        static IEnumerable<IToken> TransformToPolishNotation(List<IToken> infixTokenList)
        {
            Queue<IToken> queue = new Queue<IToken>();
            Stack<IToken> stack = new Stack<IToken>();

            int index = 0;
            while (infixTokenList.Count > index)
            {
                IToken t = infixTokenList[index];

                switch (t.Type)
                {
                    case TokenType.Literal:
                    case TokenType.Property:
                        queue.Enqueue(t);
                        break;
                    case TokenType.Operator:
                    case TokenType.Function:
                        stack.Push(t);
                        break;
                    case TokenType.End:
                    default:
                        break;
                }

                ++index;
            }
            while (stack.Count > 0)
            {
                queue.Enqueue(stack.Pop());
            }
            return queue.Reverse().ToList();
        }

        static Expression BuildExpressionTree(ref List<IToken>.Enumerator enumerator, ref ParameterExpression param)
        {
            if (enumerator.MoveNext())
            {
                switch (enumerator.Current.Type)
                {
                    case TokenType.Literal:
                        {
                            ConstantExpression constant = null;
                            if (enumerator.Current.Value.StartsWith("'") && enumerator.Current.Value.EndsWith("'"))
                            {
                                string str = enumerator.Current.Value.Trim('\'');
                                constant = Expression.Constant(str, typeof(string));
                            }
                            else
                            {
                                int num = Int32.Parse(enumerator.Current.Value);
                                constant = Expression.Constant(num, typeof(int));
                            }
                            return constant;
                        }
                    case TokenType.Function:
                    {
                        string value = enumerator.Current.Value;
                        MethodInfo method = typeof(string).GetMethod(value, new[] { typeof(string) });
                        Expression right = BuildExpressionTree(ref enumerator, ref param);
                        Expression left = BuildExpressionTree(ref enumerator, ref param);
                        MethodCallExpression funcParam = Expression.Call(left, method, right);
                        return funcParam;
                    }
                    case TokenType.Property:
                        {
                            string value = enumerator.Current.Value;
                            Expression obj = BuildExpressionTree(ref enumerator, ref param);
                            MemberExpression me = null;
                            if (string.Equals(value, ".year", StringComparison.OrdinalIgnoreCase))
                                me = Expression.Property(obj, "Year");
                            else if (string.Equals(value, ".title", StringComparison.OrdinalIgnoreCase))
                                me = Expression.Property(obj, "Title");
                            return me;
                        }
                    case TokenType.Operator:
                        {
                            string value = enumerator.Current.Value;
                            Expression right = BuildExpressionTree(ref enumerator, ref param);
                            Expression left = BuildExpressionTree(ref enumerator, ref param);
                            BinaryExpression expr = null;
                            if (value == "<")
                                expr = Expression.LessThan(left, right);
                            else if (value == ">")
                                expr = Expression.GreaterThan(left, right);
                            else if (value == "=")
                                expr = Expression.Equal(left, right);
                            else if (value == ">=")
                                expr = Expression.GreaterThanOrEqual(left, right);
                            else if (value == "<=")
                                expr = Expression.LessThanOrEqual(left, right);
                            return expr;
                        }
                    case TokenType.End:
                    default:
                        return null;
                }
            }

            param = Expression.Parameter(typeof(Film), "film");
            return param;
        }

        static void Main(string[] args)
        {
            var films = SeedData();
            Console.WriteLine("----");
            Console.WriteLine("Enter the request: ");
            string query = Console.ReadLine();
            var tokens = Tokenize(query);
            var polished = TransformToPolishNotation(tokens.ToList());
            var plist = polished.ToList();
            plist.Add(tokens.Last());

            var enumerator = polished.ToList().GetEnumerator();
            ParameterExpression pe = null;
            var expr = BuildExpressionTree(ref enumerator, ref pe);
            var check = Expression.Lambda<Func<Film, bool>>(expr, new[] { pe }).Compile();

            foreach (var film in films)
            {
                bool res = check(film);
                Console.WriteLine($"Title '{film.Title}', Year '{film.Year}' ----> {res}");
            }

            Console.WriteLine("---");
        }
    }
}
