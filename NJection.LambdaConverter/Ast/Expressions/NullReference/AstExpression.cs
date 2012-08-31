using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static NullReference NullReference(NRefactory.NullReferenceExpression nullReferenceExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new NullReference(nullReferenceExpression, scope, visitor);
        }
    }
}