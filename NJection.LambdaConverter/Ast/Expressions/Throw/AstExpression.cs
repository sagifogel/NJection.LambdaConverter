using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Throw Throw(NRefactory.ThrowStatement throwStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Throw(throwStatement, scope, visitor);
        }
    }
}