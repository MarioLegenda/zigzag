namespace ZigZag.Parser;

using Lexer;
using Token;
using Ast;

public class Parser
{
    private Lexer Lexer { get; set; }
    private List<string> _errors = new();

    private Token _currentToken;
    private Token _peekToken;

    public Parser(Lexer lexer)
    {
        this.Lexer = lexer;
        
        this.nextToken();
        this.nextToken();
    }

    public Program ParseProgram()
    {
        Program program = new Program();

        while (this._currentToken.Type != Tokens.EOF)
        {
            IStatement? stmt = this.parseStatement();
            if (stmt != null)
            {
                program.Statements.Add(stmt);
            }

            this.nextToken();
        }

        return program;
    }

    public List<string> Errors()
    {
        return this._errors;
    }

    private void nextToken()
    {
        this._currentToken = this._peekToken;
        this._peekToken = this.Lexer.NextToken();
    }

    private IStatement? parseStatement()
    {
        switch (this._currentToken.Type)
        {
            case Tokens.LET:
                return parseLetStatement();
            case Tokens.RETURN:
                return parseReturnStatement();
            default:
                return null;
        }
    }

    private ReturnStatement? parseReturnStatement()
    {
        ReturnStatement returnStatement = new ReturnStatement();
        returnStatement.Token = this._currentToken;
        
        this.nextToken();
        
        // TODO: We're skipping the expressions until we
        // encounter a semicolon
        while (!this.curTokenIs(Tokens.SEMICOLON))
        {
            this.nextToken();
        }
        
        return returnStatement;
    }

    private LetStatement? parseLetStatement()
    {
        LetStatement letStatement = new LetStatement();
        letStatement.Token = this._currentToken;

        if (!this.expectPeek(Tokens.IDENT))
        {
            Console.WriteLine("Enters token ident");
            return null;
        }

        letStatement.Name = new Identifier(this._currentToken, this._currentToken.Literal);

        if (!this.expectPeek(Tokens.ASSIGN))
        {
            Console.WriteLine("Enters peek assign");
            return null;
        }

        // TODO: We're skipping the expressions until we
        // encounter a semicolon
        while (!this.curTokenIs(Tokens.SEMICOLON))
        {
            this.nextToken();
        }
        
        return letStatement;
    }

    private bool curTokenIs(string token)
    {
        return this._currentToken.Type == token;
    }
    
    private bool peekTokenIs(string token)
    {
        return this._peekToken.Type == token;
    }

    private bool expectPeek(string token)
    {
        if (this.peekTokenIs(token))
        {
            this.nextToken();
            return true;
        }

        this.peekError(token);
        return false;
    }

    private void peekError(string token)
    {
        string msg = $"Expected next token to be {token}, got {this._peekToken.Type}";
        this._errors.Add(msg);
    }
}