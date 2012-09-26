using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Invocation Invocation(NRefactory.InvocationExpression invocationExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Invocation(invocationExpression, scope, visitor);
        }
    }
}