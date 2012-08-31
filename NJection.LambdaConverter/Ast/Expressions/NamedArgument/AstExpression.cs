using NJection.LambdaConverter.Visitors;
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