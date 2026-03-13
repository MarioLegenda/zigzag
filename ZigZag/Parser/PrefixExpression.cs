namespace ZigZag.Parser;

using Token;
using ZigZag.Ast;

public class PrefixExpression: IPrefixParser
{
    public Token Token { get; set; }
    public IExpression Parse(Token token, Parser parser)
    {
        Ast.PrefixExpression expression = new Ast.PrefixExpression();
        expression.Token = token;
        expression.Operator = token.Literal;

        parser.NextToken();

       expression.Right = parser.ParseExpression(ParsingTokens.PREFIX);
        
        return expression;
    }
}