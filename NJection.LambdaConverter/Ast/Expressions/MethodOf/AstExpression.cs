using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static MethodOf MethodOf(NRefactory.InvocationExpression invocationExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new MethodOf(invocationExpression, scope, visitor);
        }
    }
}