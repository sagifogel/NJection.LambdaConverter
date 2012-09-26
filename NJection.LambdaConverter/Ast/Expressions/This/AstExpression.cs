using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static This This(NRefactory.ThisReferenceExpression thisReferenceExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new This(thisReferenceExpression, scope, visitor);
        }
    }
}