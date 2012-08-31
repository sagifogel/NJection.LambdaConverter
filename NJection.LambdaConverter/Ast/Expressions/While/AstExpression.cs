using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static While While(NRefactory.WhileStatement whileStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new While(whileStatement, scope, visitor);
        }
    }
}