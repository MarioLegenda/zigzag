namespace ZigZag.Parser;

using Lexer;
using Token;
using Ast;

public class Parser
{
    private Lexer Lexer { get; set; }

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
            default:
                return null;
        }
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

        return false;
    }
}