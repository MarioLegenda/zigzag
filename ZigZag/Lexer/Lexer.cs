namespace ZigZag.Lexer;

using ZigZag.Token;

public class Lexer
{
    private string Input { get; set; }
    private int Position { get; set; }
    private int ReadPosition { get; set; }
    private byte Ch { get; set; }
    
    public Lexer(string input)
    {
        this.Input = input;
        this.readChar();
    }

    private void readChar()
    {
        if (this.ReadPosition >= this.Input.Length)
        {
            this.Ch = 0;
        }
        else
        {
            this.Ch = (byte)this.Input[this.ReadPosition];
        }

        this.Position = this.ReadPosition;
        this.ReadPosition += 1;
    }

    public Token NextToken()
    {
        this.skipWhitespace();

        switch (this.Ch)
        {
            case 61:
                Token t1 = new Token(Tokens.ASSIGN, ((char)this.Ch).ToString());
                this.readChar();
                return t1;
            case 59:
                Token t2 = new Token(Tokens.SEMICOLON, ((char)this.Ch).ToString());
                this.readChar();
                return t2;
            case 40:
                Token t3 = new Token(Tokens.LPAREN, ((char)this.Ch).ToString());
                this.readChar();
                return t3;
            case 41:
                Token t4 = new Token(Tokens.RPAREN, ((char)this.Ch).ToString());
                this.readChar();
                return t4;
            case 44:
                Token t5 = new Token(Tokens.COMMA, ((char)this.Ch).ToString());
                this.readChar();
                return t5;
            case 43:
                Token t6 = new Token(Tokens.PLUS, ((char)this.Ch).ToString());
                this.readChar();
                return t6;
            case 123:
                Token t7 = new Token(Tokens.LBRACE, ((char)this.Ch).ToString());
                this.readChar();
                return t7;
            case 125:
                Token t8 = new Token(Tokens.RBRACE, ((char)this.Ch).ToString());
                this.readChar();
                return t8;
            case 0:
                Token eof = new Token(Tokens.EOF, ((char)this.Ch).ToString());
                return eof;
            default:
                if (isLetter((char)this.Ch))
                {
                    string identifier = this.readIdentifier();
                    Token tok = new Token(Keywords.Lookup(identifier), identifier);
                    return tok;
                }
                else if (isDigit((char)this.Ch))
                {
                    Token tok = new Token(Tokens.INT, this.readNumber());
                    this.readChar();
                    return tok;
                }
                else
                {
                    return new Token(Tokens.ILLEGAL, this.Ch.ToString());
                }
        }
    }

    private string readIdentifier()
    {
        int position = this.Position;

        while (isLetter((char)this.Ch))
        {
            this.readChar();
        }

        return this.Input.Substring(position, this.Position - position);
    }

    private string readNumber()
    {
        int position = this.Position;

        while (isDigit((char)this.Ch))
        {
            this.readChar();
        }

        return this.Input.Substring(position, this.Position - position);
    }

    private bool isLetter(char ch)
    {
        return 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_';
    }

    private bool isDigit(char ch)
    {
        return '0' <= ch && ch <= '9';
    }

    private void skipWhitespace()
    {
        while(this.Ch == ' ' || this.Ch == '\t' || this.Ch == '\n' || this.Ch == '\r')
        {
            this.readChar();
        }
    }
}