namespace ZigZag.Parser;

using ZigZag.Ast;
using Token;

public class InfixParser: IInfixParser
{
    public IExpression Parse(Token token, IExpression left, Parser parser)
    {
        InfixExpression infixExpression = new InfixExpression();
        infixExpression.Token = token;
        infixExpression.Operator = token.Literal;
        infixExpression.Left = left;

        ParsingTokens precendence = parser.currentPrecendence();
        parser.NextToken();
        infixExpression.Right = parser.ParseExpression(precendence);

        return infixExpression;
    }
}