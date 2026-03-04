namespace ZigZag.Lexer;

using ZigZag.Token;

public class Lexer
{
    public Lexer(string input)
    {
        this.input = input;
        this.readChar();
    }
    
    private string input { get; set; }
    private int position { get; set; }
    private int readPosition { get; set; }
    private char ch { get; set; }

    private void readChar()
    {
        if (this.readPosition >= this.input.Length)
        {
            this.ch = ' ';
        }
        else
        {
            this.ch = this.input[this.readPosition];
        }

        this.position = this.readPosition;
        this.readPosition += 1;
    }

    public Token NextToken()
    {
        switch (this.ch)
        {
            case '=':
                Token t1 = new Token(Tokens.ASSIGN, this.ch);
                this.readChar();
                return t1;
            case ';':
                Token t2 = new Token(Tokens.SEMICOLON, this.ch);
                this.readChar();
                return t2;
            case '(':
                Token t3 = new Token(Tokens.LPAREN, this.ch);
                this.readChar();
                return t3;
            case ')':
                Token t4 = new Token(Tokens.RPAREN, this.ch);
                this.readChar();
                return t4;
            case ',':
                Token t5 = new Token(Tokens.COMMA, this.ch);
                this.readChar();
                return t5;
            case '+':
                Token t6 = new Token(Tokens.PLUS, this.ch);
                this.readChar();
                return t6;
            case '{':
                Token t7 = new Token(Tokens.LBRACE, this.ch);
                this.readChar();
                return t7;
            case '}':
                Token t8 = new Token(Tokens.RBRACE, this.ch);
                this.readChar();
                return t8;
            default:
                Token eof = new Token(Tokens.EOF, this.ch);
                return eof;
        }
    }
}