namespace ZigZag.Lexer;

using ZigZag.Token;

public class Lexer
{
    /**
     * Input is the current input 
     */
    private string Input { get; }
    /**
     * Position is the current character in the input
     */
    private int Position { get; set; }
    /**
     * ReadPosition is the next character in the input
     */
    private int ReadPosition { get; set; }
    /**
     * Current character as a byte
     */
    private byte Ch { get; set; }
    
    public Lexer(string input)
    {
        this.Input = input;
        this.readChar();
    }

    /**
     * Used by the parser to get the next token from the input. 
     */
    public Token NextToken()
    {
        this.skipWhitespace();

        switch (this.Ch)
        {
            case 61:
                /**
                 * If the token is a = sign, check if the next token is a = sign and then
                 * create a Tokens.EQ token. If not, it is an assign token.
                 */
                if (this.peekChar() == 61)
                {
                    byte c = this.Ch;
                    this.readChar();
                    Token t15 = new Token(Tokens.EQ, ((char)this.Ch).ToString() + ((char) c).ToString());
                    this.readChar();
                    return t15;
                }
                else
                {
                    Token t1 = new Token(Tokens.ASSIGN, ((char)this.Ch).ToString());
                    this.readChar();
                    return t1;
                }
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
            case 45:
                Token t9 = new Token(Tokens.MINUS, ((char)this.Ch).ToString());
                this.readChar();
                return t9;
            case 33:
                /**
                 * Current character is !, if the next character is a =, then it is a Token.NOT_EQ,
                 * if it is not, then it is Tokens.BANG. 
                 */
                if (this.peekChar() == 61)
                {
                    byte c = this.Ch;
                    this.readChar();
                    Token t15 = new Token(Tokens.NOT_EQ, ((char)c).ToString() + ((char) this.Ch).ToString());
                    this.readChar();
                    return t15;
                }
                else
                {
                    Token t1 = new Token(Tokens.BANG, ((char)this.Ch).ToString());
                    this.readChar();
                    return t1;
                }
            case 42:
                Token t11 = new Token(Tokens.ASTERIX, ((char)this.Ch).ToString());
                this.readChar();
                return t11;
            case 47:
                Token t12 = new Token(Tokens.SLASH, ((char)this.Ch).ToString());
                this.readChar();
                return t12;
            case 60:
                Token t13 = new Token(Tokens.LT, ((char)this.Ch).ToString());
                this.readChar();
                return t13;
            case 62:
                Token t14 = new Token(Tokens.GT, ((char)this.Ch).ToString());
                this.readChar();
                return t14;
            case 0:
                Token eof = new Token(Tokens.EOF, ((char)this.Ch).ToString());
                return eof;
            case 34:
                Token t16 = new Token(Tokens.STRING, this.readString());
                this.readChar();
                return t16;
            case 91:
                Token t17 = new Token(Tokens.LBRACKET, ((char)this.Ch).ToString());
                this.readChar();
                return t17;
            case 93:
                Token t18 = new Token(Tokens.RBRACKET, ((char)this.Ch).ToString());
                this.readChar();
                return t18;
            case 58:
                Token t19 = new Token(Tokens.COLON, ((char)this.Ch).ToString());
                this.readChar();
                return t19;
            default:
                /**
                 * If the token is a letter, then it is keyword. This keyword is
                 * traversed until complete in the this.readIdentifier() function
                 */
                if (isLetter((char)this.Ch))
                {
                    string identifier = this.readIdentifier();
                    Token tok = new Token(Keywords.Lookup(identifier), identifier);
                    return tok;
                }
                else if (isDigit((char)this.Ch))
                {
                    Token tok = new Token(Tokens.INT, this.readNumber());
                    return tok;
                }
                else
                {
                    return new Token(Tokens.ILLEGAL, this.Ch.ToString());
                }
        }
    }
    
    /**
     * Reads the next character in the input, one by one. 
     */
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

    private string readString()
    {
        int position = this.Position + 1;

        for (;;)
        {
            this.readChar();

            if (this.Ch == 34 || this.Ch == 0)
            {
                break;
            }
        }

        return this.Input.Substring(position, this.Position - position);
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

    private byte peekChar()
    {
        if (this.ReadPosition >= this.Input.Length)
        {
            return 0;
        }
        else
        {
            return (byte)this.Input[this.ReadPosition];
        }
    }
}