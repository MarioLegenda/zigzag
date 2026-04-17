namespace ZigZag.Parser;

using ZigZag.Ast;
using Token;

public class InfixParser: IInfixParser
{
    /**
     * InfixParser always gets called with the current token as the operator (+, -, * etc...) and peek token
     * as either a IntegerLiteral, BoolLiteral or something else expect the operators. 
     */
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