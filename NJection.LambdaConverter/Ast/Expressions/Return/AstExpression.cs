using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Return Return(NRefactory.ReturnStatement returnStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Return(returnStatement, scope, visitor);
        }
    }
}