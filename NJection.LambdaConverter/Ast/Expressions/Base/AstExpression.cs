using NJection.LambdaConverter.Visitors;
using NJection.Scope;
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