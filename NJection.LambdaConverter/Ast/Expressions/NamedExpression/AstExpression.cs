using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static NamedExpression NamedExpression(NRefactory.NamedExpression namedExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new NamedExpression(namedExpression, scope, visitor);
        }
    }
}