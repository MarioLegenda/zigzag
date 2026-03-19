namespace ZigZag.Parser;

using Lexer;
using Token;
using Ast;

public class Parser: BaseParser
{
    public Parser(Lexer lexer): base(lexer)
    {
        this.Lexer = lexer;
        
        this.NextToken();
        this.NextToken();

        this.prefixParsers[Tokens.IDENT] = new IdentifierParser();
        this.prefixParsers[Tokens.INT] = new IntegerParser();
        this.prefixParsers[Tokens.BANG] = new PrefixExpression();
        this.prefixParsers[Tokens.MINUS] = new PrefixExpression();
        
        this.prefixParsers[Tokens.TRUE] = new BooleanParser();
        this.prefixParsers[Tokens.FALSE] = new BooleanParser();
        
        this._infixParsers[Tokens.MINUS] = new InfixParser();
        this._infixParsers[Tokens.PLUS] = new InfixParser();
        this._infixParsers[Tokens.SLASH] = new InfixParser();
        this._infixParsers[Tokens.ASTERIX] = new InfixParser();
        this._infixParsers[Tokens.EQ] = new InfixParser();
        this._infixParsers[Tokens.NOT_EQ] = new InfixParser();
        this._infixParsers[Tokens.LT] = new InfixParser();
        this._infixParsers[Tokens.GT] = new InfixParser();
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

            this.NextToken();
        }

        return program;
    }
    
    public IExpression ParseExpression(ParsingTokens precedence)
    {
        if (!this.prefixParsers.ContainsKey(this._currentToken.Type))
        {
            throw new Exception($"No prefix parser for {this._currentToken.Type}");
        }

        IPrefixParser parser = this.prefixParsers[this._currentToken.Type];
        IExpression leftExp = parser.Parse(this._currentToken, this);

        while (!this.peekTokenIs(Tokens.SEMICOLON) && precedence < this.peekPrecendence())
        {
            if (!this._infixParsers.ContainsKey(this._peekToken.Type))
            {
                return leftExp;
            }

            IInfixParser infixParser = this._infixParsers[this._peekToken.Type];

            this.NextToken();

            leftExp = infixParser.Parse(this._currentToken, leftExp, this);
        }

        return leftExp;
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
        expressionStatement.Expression = this.ParseExpression(ParsingTokens.LOWEST);

        if (this.peekTokenIs(Tokens.SEMICOLON))
        {
            this.NextToken();
        }
        
        return expressionStatement;
    }

    public ParsingTokens peekPrecendence()
    {
        if (Precendences.precedences.ContainsKey(this._peekToken.Literal))
        {
            return Precendences.precedences[this._peekToken.Literal];
        }

        return ParsingTokens.LOWEST;
    }

    public ParsingTokens currentPrecendence()
    {
        if (Precendences.precedences.ContainsKey(this._currentToken.Literal))
        {
            return Precendences.precedences[this._currentToken.Literal];
        }

        return ParsingTokens.LOWEST;
    }

    private ReturnStatement? parseReturnStatement()
    {
        ReturnStatement returnStatement = new ReturnStatement();
        returnStatement.Token = this._currentToken;
        
        this.NextToken();
        
        // TODO: We're skipping the expressions until we
        // encounter a semicolon
        while (!this.CurTokenIs(Tokens.SEMICOLON))
        {
            this.NextToken();
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
            return null;
        }

        // TODO: We're skipping the expressions until we
        // encounter a semicolon
        while (!this.CurTokenIs(Tokens.SEMICOLON))
        {
            this.NextToken();
        }
        
        return letStatement;
    }
}