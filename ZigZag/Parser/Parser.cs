namespace ZigZag.Parser;

using Lexer;
using Token;
using Ast;

public class Parser: BaseParser
{
    public Parser(Lexer lexer): base(lexer)
    {
        this.Lexer = lexer;
        
        this.nextToken();
        this.nextToken();

        this.prefixParsers[Tokens.IDENT] = new IdentifierParser();
        this.prefixParsers[Tokens.INT] = new IntegerParser();
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

    private IStatement? parseStatement()
    {
        switch (this._currentToken.Type)
        {
            case Tokens.LET:
                return parseLetStatement();
            case Tokens.RETURN:
                return parseReturnStatement();
            default:
                return parseExpressionStatement();
        }
    }

    private ExpressionStatement parseExpressionStatement()
    {
        ExpressionStatement expressionStatement = new ExpressionStatement();
        expressionStatement.Token = this._currentToken;
        expressionStatement.Expression = this.parseExpression(ParsingTokens.LOWEST);

        if (this.peekTokenIs(Tokens.SEMICOLON))
        {
            this.nextToken();
        }
        
        return expressionStatement;
    }

    private IExpression parseExpression(ParsingTokens precendence)
    {
        if (this.prefixParsers.ContainsKey(this._currentToken.Type))
        {
            IPrefixParser? parser = this.prefixParsers[this._currentToken.Type];
            
            IExpression leftExp = parser.Parse(this._currentToken);

            return leftExp;
        }

        return null;
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

    private void parseIntegerLiteral()
    {

    }
}