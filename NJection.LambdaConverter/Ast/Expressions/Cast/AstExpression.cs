using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Cast Cast(NRefactory.CastExpression castExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Cast(castExpression, scope, visitor);
        }
    }
}