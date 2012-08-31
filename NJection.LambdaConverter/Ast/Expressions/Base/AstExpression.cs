using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Base Base(NRefactory.BaseReferenceExpression baseReferenceExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Base(baseReferenceExpression, scope, visitor);
        }
    }
}