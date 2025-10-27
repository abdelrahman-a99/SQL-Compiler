using System;
using System.Collections.Generic;

namespace SQL_Compiler.Models
{
    // ✅ Renamed to SqlToken to avoid namespace ambiguity
    public class SqlToken
    {
        public string Type { get; set; } = string.Empty;
        public string Lexeme { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }
    }

    public class Lexer
    {
        // Define SQL keywords and types
        private readonly HashSet<string> _keywords = new()
        {
            "CREATE", "TABLE", "INSERT", "INTO", "VALUES",
            "SELECT", "FROM", "WHERE", "AND", "OR", "NOT", "UPDATE", "SET", "DELETE"
        };

        private readonly HashSet<string> _types = new() { "INT", "FLOAT", "TEXT" };
        private readonly HashSet<char> _delimiters = new() { '(', ')', ',', ';' };
        private readonly HashSet<char> _operators = new() { '=', '>', '<', '+', '-', '*', '/' };

        public List<SqlToken> Analyze(string code)
        {
            var tokens = new List<SqlToken>();
            if (string.IsNullOrWhiteSpace(code)) return tokens;

            int i = 0, line = 1, col = 1;

            while (i < code.Length)
            {
                char c = code[i];

                // Skip whitespace
                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n') { line++; col = 1; }
                    else col++;
                    i++;
                    continue;
                }

                // Comments (--) or (#...#)
                if (c == '-' && i + 1 < code.Length && code[i + 1] == '-')
                {
                    while (i < code.Length && code[i] != '\n') { i++; }
                    continue;
                }
                if (c == '#')
                {
                    i++;
                    while (i < code.Length && code[i] != '#') { i++; }
                    if (i < code.Length) i++;
                    continue;
                }

                // Identifiers, keywords, or types
                if (char.IsLetter(c))
                {
                    int start = i, startCol = col;
                    while (i < code.Length && (char.IsLetterOrDigit(code[i]) || code[i] == '_'))
                    { i++; col++; }

                    string word = code.Substring(start, i - start);
                    string upper = word.ToUpper();

                    if (_keywords.Contains(upper))
                        tokens.Add(new SqlToken { Type = upper, Lexeme = word, Line = line, Column = startCol });
                    else if (_types.Contains(upper))
                        tokens.Add(new SqlToken { Type = "TYPE", Lexeme = word, Line = line, Column = startCol });
                    else
                        tokens.Add(new SqlToken { Type = "IDENTIFIER", Lexeme = word, Line = line, Column = startCol });

                    continue;
                }

                // Numbers
                if (char.IsDigit(c))
                {
                    int start = i, startCol = col;
                    while (i < code.Length && char.IsDigit(code[i])) { i++; col++; }
                    string num = code.Substring(start, i - start);
                    tokens.Add(new SqlToken { Type = "NUMBER", Lexeme = num, Line = line, Column = startCol });
                    continue;
                }

                // Strings (single quotes)
                if (c == '\'')
                {
                    int startCol = col;
                    i++; col++;
                    int start = i;
                    while (i < code.Length && code[i] != '\'') { i++; col++; }
                    string str = code.Substring(start, i - start);
                    tokens.Add(new SqlToken { Type = "STRING", Lexeme = $"'{str}'", Line = line, Column = startCol });
                    if (i < code.Length && code[i] == '\'') { i++; col++; }
                    continue;
                }

                // Operators
                if (_operators.Contains(c))
                {
                    tokens.Add(new SqlToken { Type = "OPERATOR", Lexeme = c.ToString(), Line = line, Column = col });
                    i++; col++;
                    continue;
                }

                // Delimiters
                if (_delimiters.Contains(c))
                {
                    string type = c switch
                    {
                        '(' => "LEFT_PAREN",
                        ')' => "RIGHT_PAREN",
                        ',' => "COMMA",
                        ';' => "SEMICOLON",
                        _ => "DELIMITER"
                    };

                    tokens.Add(new SqlToken { Type = type, Lexeme = c.ToString(), Line = line, Column = col });
                    i++; col++;
                    continue;
                }

                // Invalid character
                tokens.Add(new SqlToken { Type = "ERROR", Lexeme = $"Invalid char '{c}'", Line = line, Column = col });
                i++; col++;
            }

            return tokens;
        }
    }
}
