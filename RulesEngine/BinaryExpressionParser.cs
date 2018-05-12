using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulesEngine
{
    public static class BinaryExpressionParser
    {
        private const string AND = "&";
        private const string OR = "|";

        private static readonly Dictionary<string, Token> operators = new List<Token>
        {
            new Token { Symbol = AND, Precedence = 11, RightAssociative = false },
            new Token { Symbol = OR, Precedence = 12, RightAssociative = false },

        }.ToDictionary(op => op.Symbol);

        /// <summary>
        /// Separate a binary expression string into its tokens.
        /// </summary>
        /// <remarks>
        /// This method will only parse a logical binary expression containing "&", "|", "true", "false", "(", and ")"
        /// </remarks>
        /// <param name="infix">The infix notation of the expression.</param>
        /// <returns>An enumerable of Token objects representing each token</returns>
        public static IEnumerable<Token> Tokenize(string infix)
        {
            using (TextReader reader = new StringReader(infix))
            {
                var token = new StringBuilder();

                int curr;
                while ((curr = reader.Read()) != -1)
                {
                    char ch = (char)curr;
                    TokenType currType = DetermineType(ch);
                    if (currType == TokenType.WhiteSpace)
                        continue;

                    token.Append(ch);

                    // See if the next character is going to add to the current token
                    int next = reader.Peek();
                    TokenType nextType = next != -1 ? DetermineType((char)next) : TokenType.WhiteSpace;
                    
                    if (currType == nextType)
                        continue;

                    string tokStr = token.ToString();

                    if (currType == TokenType.Boolean && !bool.TryParse(tokStr, out _))
                        throw new FormatException($"Invalid token encountered: {tokStr}");

                    yield return new Token
                    {
                        Type = next == '(' ? TokenType.Function : currType,
                        Symbol = tokStr
                    };

                    token.Clear();
                }
            }
        }

        /// <summary>
        /// Converts an enumerable of infix tokens to the postfix order using the Shunting-Yard algorithm
        /// </summary>
        /// <param name="tokens">Tokens in infix order</param>
        /// <returns>Tokens in postfix order (Reverse Polish Notation)</returns>
        public static IEnumerable<Token> ToPostfixOrder(IEnumerable<Token> tokens)
        {
            var stack = new Stack<Token>();
            foreach (Token tok in tokens)
            {
                switch (tok.Type)
                {
                    // Lowest level, can be added as-is
                    case TokenType.Boolean:
                    case TokenType.Number:
                        yield return tok;
                        break;
                    case TokenType.Function:
                        stack.Push(tok);
                        break;
                    // Order the operators according to their precedence
                    case TokenType.Operator:
                        while (stack.Any() && stack.Peek().Type == TokenType.Operator && CompareOperators(tok, stack.Peek()))
                            yield return stack.Pop();
                        stack.Push(tok);
                        break;
                    case TokenType.Parenthesis:
                        if (tok.Symbol == "(")
                            stack.Push(tok);
                        else
                        {
                            while (stack.Peek().Symbol != "(")
                                yield return stack.Pop();
                            stack.Pop();
                            if (stack.Peek().Type == TokenType.Function)
                                yield return stack.Pop();
                        }
                        break;
                    default:
                        throw new FormatException($"Cannot identify token: {tok.Symbol}");
                }
            }
            // Return the stack in reverse order
            while (stack.Any())
            {
                var tok = stack.Pop();
                if (tok.Type == TokenType.Parenthesis)
                    throw new FormatException($"Mismatched parentheses");
                yield return tok;
            }
        }

        /// <summary>
        /// Convert the given infix string to postfix and evaluate the result of the logical expression
        /// </summary>
        /// <param name="infix">Infix string to evaluate</param>
        /// <returns>The result of the logical expression</returns>
        public static bool ConvertAndEvaluate(string infix)
        {
            // Split the infix string into its tokens
            var tokens = Tokenize(infix);
            // Change the order of the infix tokens to postfix
            var postfix = ToPostfixOrder(tokens);

            return EvaluatePostfixBinary(postfix);
        }

        /// <summary>
        /// Evaluate a binary expression represented as a series of tokens in postfix order.
        /// </summary>
        /// <param name="tokens">Tokens in postfix order</param>
        /// <returns>Result of the expression</returns>
        public static bool EvaluatePostfixBinary(IEnumerable<Token> tokens)
        {
            Stack<string> evalStack = new Stack<string>();

            foreach (Token tok in tokens)
            {
                bool op1, op2;
                switch (tok.Symbol)
                {
                    case AND:
                        op1 = Convert.ToBoolean(evalStack.Pop());
                        op2 = Convert.ToBoolean(evalStack.Pop());
                        evalStack.Push((op1 && op2).ToString());
                        break;
                    case OR:
                        op1 = Convert.ToBoolean(evalStack.Pop());
                        op2 = Convert.ToBoolean(evalStack.Pop());
                        evalStack.Push((op1 || op2).ToString());
                        break;
                    default:
                        evalStack.Push(tok.Symbol);
                        break;
                }
            }

            return Convert.ToBoolean(evalStack.Pop());
        }

        // Compare precendence and associativity of tokens to determine which takes priority
        private static bool CompareOperators(Token op1, Token op2)
        {
            return op1.RightAssociative ? op1.Precedence < op2.Precedence : op1.Precedence <= op2.Precedence;
        }

        // Determine the token type of a character
        private static TokenType DetermineType(char ch)
        {
            if (char.IsLetter(ch))
                return TokenType.Boolean;

            if (char.IsDigit(ch))
                return TokenType.Number;

            if (char.IsWhiteSpace(ch))
                return TokenType.WhiteSpace;

            if (ch == '(' || ch == ')')
                return TokenType.Parenthesis;

            if (operators.ContainsKey(Convert.ToString(ch)))
                return TokenType.Operator;

            throw new FormatException($"Cannot classify character: '{ch}' in input.");
        }

        public static string Replace<TKey, TValue>(this string str, IDictionary<TKey, TValue> findAndReplace)
        {
            string retString = str;

            foreach (var replacement in findAndReplace)
            {
                retString = retString.Replace(replacement.Key.ToString(), replacement.Value.ToString());
            }

            return retString;
        }
    }

    public class Token
    {
        public string Symbol { get; set; }

        public int Precedence { get; set; }

        public bool RightAssociative { get; set; }

        public TokenType Type { get; set; }
    }

    public enum TokenType
    {
        Number,
        Operator,
        Parenthesis,
        WhiteSpace,
        Function,
        Boolean
    }
}
