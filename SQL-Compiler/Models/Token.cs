namespace SQL_Compiler.Models
{
    public enum TokenType
    {
        KEYWORD,
        IDENTIFIER,
        NUMBER,
        STRING,
        OPERATOR,
        DELIMITER,
        TYPE,
        ERROR
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Lexeme { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }

        public override string ToString()
        {
            return $"Token: {Type}, Lexeme: {Lexeme}, (line {Line}, col {Column})";
        }
    }
}
