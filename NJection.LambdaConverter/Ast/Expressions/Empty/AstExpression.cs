using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Empty Empty(NRefactory.EmptyExpression emptyExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Empty(emptyExpression, scope, visitor);
        }
    }
}