using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static NamedArgument NamedArgument(NRefactory.NamedArgumentExpression namedArgumentExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new NamedArgument(namedArgumentExpression, scope, visitor);
        }
    }
}