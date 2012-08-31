using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Direction Direction(NRefactory.DirectionExpression directionExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Direction(directionExpression, scope, visitor);
        }
    }
}