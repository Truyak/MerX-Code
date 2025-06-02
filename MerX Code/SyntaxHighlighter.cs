using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace MerX_Code
{
    // Token types
    public enum TokenType
    {
        Keyword,
        Identifier,
        Number,
        String,
        Operator,
        Whitespace,
        Comment,
        Unknown
    }

    // Token structure
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }

        public Token(TokenType type, string value, int start, int length)
        {
            Type = type;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Start = start;
            Length = length;
        }

        public override string ToString()
        {
            return $"[{Type}] \"{Value}\" (Start: {Start}, Length: {Length})";
        }
    }

    // Theme management
    public class ThemeManager
    {
        public class Theme
        {
            public string Name { get; set; }
            public Dictionary<TokenType, Brush> Colors { get; set; }
            public Brush Background { get; set; }
            public Brush Foreground { get; set; }

            public Theme(string name, Dictionary<TokenType, Brush> colors, Brush background, Brush foreground)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Colors = colors ?? throw new ArgumentNullException(nameof(colors));
                Background = background ?? throw new ArgumentNullException(nameof(background));
                Foreground = foreground ?? throw new ArgumentNullException(nameof(foreground));
            }
        }

        public static Theme DarkTheme = new Theme(
            name: "Dark",
            colors: new Dictionary<TokenType, Brush>
            {
                { TokenType.Keyword, Brushes.Cyan },
                { TokenType.Identifier, Brushes.White },
                { TokenType.Number, Brushes.Yellow },
                { TokenType.String, Brushes.Orange },
                { TokenType.Operator, Brushes.Magenta },
                { TokenType.Whitespace, Brushes.Transparent },
                { TokenType.Comment, Brushes.Green },
                { TokenType.Unknown, Brushes.Red }
            },
            background: Brushes.Black,
            foreground: Brushes.White
        );

        public static Theme LightTheme = new Theme(
            name: "Light",
            colors: new Dictionary<TokenType, Brush>
            {
                { TokenType.Keyword, Brushes.DarkMagenta },
                { TokenType.Identifier, Brushes.Black },
                { TokenType.Number, Brushes.DarkGreen },
                { TokenType.String, Brushes.Chocolate },
                { TokenType.Operator, Brushes.DarkViolet },
                { TokenType.Whitespace, Brushes.Transparent },
                { TokenType.Comment, Brushes.Gray },    
                { TokenType.Unknown, Brushes.Red }
            },
            background: Brushes.White,
            foreground: Brushes.Black
        );

        public static Theme CurrentTheme = DarkTheme;
    }

    // Lexical Analyzer
    // Lexical Analyzer
    public class Lexer
    {
        private readonly string input;
        private int position;
        private static readonly HashSet<string> keywords = new HashSet<string>
    {
        "if", "else", "while", "for", "return", "int", "string", "void", "class", "public", "private", "static", "new", "this", "null", "true", "false", "break", "continue", "switch", "case", "default", "try", "catch", "finally", "throw", "bool"
    };
        private static readonly HashSet<char> operators = new HashSet<char>
    {
        '+', '-', '*', '/', '=', '>', '<', '!', '&', '|', ';', '(', ')', '{', '}', '[', ']', ','
    };


        public Lexer(string input)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            position = 0;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            position = 0; // Her tokenize çağrıldığında baştan başla
            while (position < input.Length)
            {
                char current = input[position];

                if (char.IsWhiteSpace(current))
                {
                    tokens.Add(ReadWhitespace());
                }
                else if (current == '/')
                {
                    if (position + 1 < input.Length && input[position + 1] == '/')
                    {
                        tokens.Add(ReadSingleLineComment());
                    }
                    else if (position + 1 < input.Length && input[position + 1] == '*')
                    {
                        tokens.Add(ReadMultiLineComment());
                    }
                    else
                    {
                        tokens.Add(ReadOperator());
                    }
                }
                else if (char.IsLetter(current) || current == '_')
                {
                    tokens.Add(ReadIdentifierOrKeyword());
                }
                else if (char.IsDigit(current))
                {
                    tokens.Add(ReadNumber());
                }
                else if (current == '"')
                {
                    tokens.Add(ReadString());
                }
                else if (operators.Contains(current))
                {
                    tokens.Add(ReadOperator());
                }
                else
                {
                    tokens.Add(new Token(TokenType.Unknown, current.ToString(), position, 1));
                    position++;
                }
            }
            return tokens;
        }

        private Token ReadWhitespace()
        {
            int start = position;
            while (position < input.Length && char.IsWhiteSpace(input[position]))
                position++;
            return new Token(TokenType.Whitespace, input.Substring(start, position - start), start, position - start);
        }

        private Token ReadIdentifierOrKeyword()
        {
            int start = position;
            while (position < input.Length && (char.IsLetterOrDigit(input[position]) || input[position] == '_'))
                position++;
            string value = input.Substring(start, position - start);
            return new Token(
                keywords.Contains(value) ? TokenType.Keyword : TokenType.Identifier,
                value,
                start,
                position - start
            );
        }

        private Token ReadNumber()
        {
            int start = position;
            while (position < input.Length && char.IsDigit(input[position]))
                position++;
            if (position < input.Length && input[position] == '.')
            {
                position++;
                while (position < input.Length && char.IsDigit(input[position]))
                    position++;
            }
            return new Token(TokenType.Number, input.Substring(start, position - start), start, position - start);
        }

        private Token ReadString()
        {
            int start = position;
            position++; // Skip opening quote
            while (position < input.Length && input[position] != '"')
            {
                if (input[position] == '\\' && position + 1 < input.Length)
                    position++;
                position++;
            }
            if (position < input.Length && input[position] == '"') // Check before skipping
                position++; // Skip closing quote
            return new Token(TokenType.String, input.Substring(start, position - start), start, position - start);
        }

        private Token ReadOperator()
        {
            int start = position;
            // Check for two-character operators first
            if (position + 1 < input.Length)
            {
                string twoCharOp = input.Substring(position, 2);
                if (new[] { "==", "!=", ">=", "<=", "&&", "||", "++", "--", "+=", "-=", "*=", "/=" }.Contains(twoCharOp))
                {
                    position += 2;
                    return new Token(TokenType.Operator, twoCharOp, start, 2);
                }
            }
            // Otherwise, it's a single-character operator
            position++;
            return new Token(TokenType.Operator, input.Substring(start, 1), start, 1);
        }

        private Token ReadSingleLineComment()
        {
            int start = position;
            position += 2; // Skip "//"
            while (position < input.Length && input[position] != '\n' && input[position] != '\r')
                position++;
            return new Token(TokenType.Comment, input.Substring(start, position - start), start, position - start);
        }

        private Token ReadMultiLineComment()
        {
            int start = position;
            position += 2; // Skip "/*"
            while (position < input.Length)
            {
                if (input[position] == '*' && position + 1 < input.Length && input[position + 1] == '/')
                {
                    position += 2; // Skip "*/"
                    break;
                }
                position++;
            }
            return new Token(TokenType.Comment, input.Substring(start, position - start), start, position - start);
        }
    }

    // Parser
    public class Parser
    {
        private readonly List<Token> tokens;
        private int position;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.Comment).ToList();
            position = 0;
        }

        private Token? CurrentToken => position < tokens.Count ? tokens[position] : null;

        private Token? ConsumeToken()
        {
            if (position < tokens.Count)
            {
                return tokens[position++];
            }
            return null;
        }

        private bool Expect(TokenType type, string? value = null)
        {
            if (CurrentToken == null || CurrentToken.Type != type)
            {
                return false;
            }
            if (value != null && CurrentToken.Value != value)
            {
                return false;
            }
            ConsumeToken();
            return true;
        }

        public bool Parse()
        {
            try
            {
                while (CurrentToken != null)
                {
                    if (!ParseStatement())
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ParseStatement()
        {
            if (CurrentToken == null) return false;

            if (CurrentToken.Type == TokenType.Keyword)
            {
                if (CurrentToken.Value == "if") return ParseIfStatement();
                if (CurrentToken.Value == "while") return ParseWhileStatement();
                if (CurrentToken.Value == "return") return ParseReturnStatement();
                if (CurrentToken.Value == "int" || CurrentToken.Value == "string" || CurrentToken.Value == "void")
                {
                    return ParseDeclarationOrAssignment();
                }
            }
            else if (CurrentToken.Type == TokenType.Identifier)
            {
                return ParseAssignment();
            }
            return false;
        }

        private bool ParseIfStatement()
        {
            if (!Expect(TokenType.Keyword, "if")) return false;
            if (!Expect(TokenType.Operator, "(")) return false;
            if (!ParseExpression()) return false;
            if (!Expect(TokenType.Operator, ")")) return false;
            if (!ParseBlockOrStatement()) return false;

            if (CurrentToken != null && CurrentToken.Type == TokenType.Keyword && CurrentToken.Value == "else")
            {
                ConsumeToken();
                if (!ParseBlockOrStatement()) return false;
            }
            return true;
        }

        private bool ParseWhileStatement()
        {
            if (!Expect(TokenType.Keyword, "while")) return false;
            if (!Expect(TokenType.Operator, "(")) return false;
            if (!ParseExpression()) return false;
            if (!Expect(TokenType.Operator, ")")) return false;
            if (!ParseBlockOrStatement()) return false;
            return true;
        }

        private bool ParseReturnStatement()
        {
            if (!Expect(TokenType.Keyword, "return")) return false;
            // Return ifadesinin opsiyonel olduğunu varsayalım, yoksa direkt noktalı virgül beklenir
            if (CurrentToken != null && CurrentToken.Type != TokenType.Operator) // Eğer ; değilse bir ifade bekleriz
            {
                if (!ParseExpression()) return false;
            }
            if (!Expect(TokenType.Operator, ";")) return false;
            return true;
        }

        private bool ParseDeclarationOrAssignment()
        {
            if (CurrentToken == null || (CurrentToken.Value != "int" && CurrentToken.Value != "string" && CurrentToken.Value != "void")) return false;
            ConsumeToken();

            if (!Expect(TokenType.Identifier)) return false;

            if (CurrentToken != null && CurrentToken.Type == TokenType.Operator && CurrentToken.Value == "=")
            {
                ConsumeToken();
                if (!ParseExpression()) return false;
            }

            if (!Expect(TokenType.Operator, ";")) return false;
            return true;
        }

        private bool ParseAssignment()
        {
            if (!Expect(TokenType.Identifier)) return false;
            if (CurrentToken != null && CurrentToken.Type == TokenType.Operator && CurrentToken.Value == "=")
            {
                ConsumeToken();
                if (!ParseExpression()) return false;
            }
            if (!Expect(TokenType.Operator, ";")) return false;
            return true;
        }

        private bool ParseExpression()
        {
            if (!ParseTerm()) return false;
            while (CurrentToken != null && CurrentToken.Type == TokenType.Operator && (CurrentToken.Value == "+" || CurrentToken.Value == "-" || CurrentToken.Value == "==" || CurrentToken.Value == "!=" || CurrentToken.Value == ">" || CurrentToken.Value == "<" || CurrentToken.Value == ">=" || CurrentToken.Value == "<="))
            {
                ConsumeToken();
                if (!ParseTerm()) return false;
            }
            return true;
        }

        private bool ParseTerm()
        {
            if (!ParseFactor()) return false;
            while (CurrentToken != null && CurrentToken.Type == TokenType.Operator && (CurrentToken.Value == "*" || CurrentToken.Value == "/"))
            {
                ConsumeToken();
                if (!ParseFactor()) return false;
            }
            return true;
        }

        private bool ParseFactor()
        {
            if (CurrentToken == null) return false;

            if (CurrentToken.Type == TokenType.Number || CurrentToken.Type == TokenType.Identifier || CurrentToken.Type == TokenType.String)
            {
                ConsumeToken();
                return true;
            }
            else if (CurrentToken.Type == TokenType.Operator && CurrentToken.Value == "(")
            {
                ConsumeToken();
                if (!ParseExpression()) return false;
                return Expect(TokenType.Operator, ")");
            }
            return false;
        }

        private bool ParseBlockOrStatement()
        {
            if (CurrentToken != null && CurrentToken.Type == TokenType.Operator && CurrentToken.Value == "{")
            {
                ConsumeToken();
                while (CurrentToken != null && (CurrentToken.Type != TokenType.Operator || CurrentToken.Value != "}"))
                {
                    if (!ParseStatement())
                    {
                        return false;
                    }
                }
                return Expect(TokenType.Operator, "}");
            }
            else
            {
                return ParseStatement();
            }
        }
    }
}