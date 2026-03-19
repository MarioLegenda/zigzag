namespace ZigZag.Parser;

using Lexer;
using Token;

public abstract class BaseParser
{
    protected Lexer Lexer { get; set; }
    protected List<string> _errors = new();

    protected Token _currentToken;
    protected Token _peekToken;
    
    protected Dictionary<string, IPrefixParser?> prefixParsers = new ();
    protected Dictionary<string, IInfixParser?> _infixParsers = new();
    
    public void NextToken()
    {
        this._currentToken = this._peekToken;
        this._peekToken = this.Lexer.NextToken();
    }

    public BaseParser(Lexer lexer)
    {
        this.Lexer = lexer;
    }
    
    public bool CurTokenIs(string token)
    {
        return this._currentToken.Type == token;
    }
    
    protected bool peekTokenIs(string token)
    {
        return this._peekToken.Type == token;
    }

    public bool expectPeek(string token)
    {
        if (this.peekTokenIs(token))
        {
            this.NextToken();
            return true;
        }

        this.peekError(token);
        return false;
    }

    protected void peekError(string token)
    {
        string msg = $"Expected next token to be {token}, got {this._peekToken.Type}";
        this._errors.Add(msg);
    }
    
    
    public List<string> Errors()
    {
        return this._errors;
    }
}