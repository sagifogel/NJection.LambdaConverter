using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Conditional Condition(NRefactory.ConditionalExpression conditionalExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Conditional(conditionalExpression, scope, visitor);
        }
    }
}