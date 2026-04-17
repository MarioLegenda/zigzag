namespace ZigZag.Parser;

using Lexer;
using Token;
using Ast;

public class Parser: BaseParser
{
    public Parser(Lexer lexer): base(lexer)
    {
        this.Lexer = lexer;
        
        /**
         * Here, we initialize _currentToken and _peekToken in BaseParser
         */
        this.NextToken();
        this.NextToken();

        this.prefixParsers[Tokens.STRING] = new StringParser();
        this.prefixParsers[Tokens.IDENT] = new IdentifierParser();
        this.prefixParsers[Tokens.INT] = new IntegerParser();
        this.prefixParsers[Tokens.BANG] = new Expression();
        this.prefixParsers[Tokens.MINUS] = new Expression();

        this.prefixParsers[Tokens.IF] = new IfParser();
        
        this.prefixParsers[Tokens.LPAREN] = new GroupedExpressionParser();
        
        this.prefixParsers[Tokens.TRUE] = new BooleanParser();
        this.prefixParsers[Tokens.FALSE] = new BooleanParser();
        
        this.prefixParsers[Tokens.FUNCTION] = new FunctionParser();

        this.prefixParsers[Tokens.LBRACKET] = new ArrayParser();

        this.prefixParsers[Tokens.LBRACE] = new HashParser();

        this._infixParsers[Tokens.MINUS] = new InfixParser();
        this._infixParsers[Tokens.PLUS] = new InfixParser();
        this._infixParsers[Tokens.SLASH] = new InfixParser();
        this._infixParsers[Tokens.ASTERIX] = new InfixParser();
        this._infixParsers[Tokens.EQ] = new InfixParser();
        this._infixParsers[Tokens.NOT_EQ] = new InfixParser();
        this._infixParsers[Tokens.LT] = new InfixParser();
        this._infixParsers[Tokens.GT] = new InfixParser();
        this._infixParsers[Tokens.LPAREN] = new CallExpressionParser();
        this._infixParsers[Tokens.LBRACKET] = new IndexParser();

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
    
    /**
     * Parsers tokens into IExpression literals
     *
     * For example, 3 + 3 * 4 / 4:
     *
     * The first thing ParseExpression does is to check if there is a prefix parser for the current token. At this
     * point, current token is '3' and peek token is '+'. It finds that there is a prefix parser which is IntegerParser.
     * That gets assigned to leftExp variable. Then, it enters the while loop.
     *
     * In the while loop, it checks if a semicolon is reached and if the current precendence passed to ParseExpression
     * (which is currently LOWEST) is lower than the peek precendence (the precendence of the peek token). It evaluates
     * to true and finds the InfixParser as the parser to parse the token. It then calls NextToken() which advances the tokens.
     * They are now: current token -> +, peek token -> 3.
     *
     * InfixParser gets called with the leftExp variable which is an IntegerLiteral. InfixParser saves the current precendence,
     * which is '+' at this point and again advances the tokens. InfixParser creates an InfixExpression which has the operator,
     * Right and Left IExpression literals. The tokens are now: current token -> + and peek token -> 3. InfixParser saves the
     * precendence of the current token (which is now '+'). InfixParser again advances the tokens which is now
     * current token -> 3, peek token -> *. With that setup, it then calls ParseExpression again the with precendence of the
     * previous current token (which was the '+' operator).
     *
     * ParseExpression (with current token -> 3 and peek token -> *). ParseExpression find the prefix parser (IntegerParser)
     * for the current token (which is 3 at this point). It then enters the while loop again which evaluates to true since
     * + < * (* is the peek token). ParseExpression advances the tokens again, so the tokens are now current token -> *
     * and peek token -> 4. It then repeats the process again for these tokens. 
     * 
     */
    public IExpression ParseExpression(ParsingTokens precedence)
    {
        if (!this.prefixParsers.ContainsKey(this._currentToken.Type))
        {
            throw new Exception($"No prefix parser for {this._currentToken.Type}");
        }

        IParser? parser = this.prefixParsers[this._currentToken.Type];
        if (parser is null)
        {
            throw new ArgumentNullException();
        }
        
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

    public BlockStatement ParseBlockStatement()
    {
        BlockStatement blockStatement = new BlockStatement();
        blockStatement.Token = this._currentToken;
        
        this.NextToken();

        while (!this.CurTokenIs(Tokens.RBRACE) && !this.CurTokenIs(Tokens.EOF))
        {
            IStatement? stmt = this.parseStatement();
            if (stmt != null)
            {
                blockStatement.Statements.Add(stmt);
            }
            
            this.NextToken();
        }

        return blockStatement;
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

        returnStatement.ReturnValue = this.ParseExpression(ParsingTokens.LOWEST);
        
        if (this.peekTokenIs(Tokens.SEMICOLON))
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

        this.NextToken();

        letStatement.Value = this.ParseExpression(ParsingTokens.LOWEST);
        
        if (this.peekTokenIs(Tokens.SEMICOLON))
        {
            this.NextToken();
        }
        
        return letStatement;
    }
}