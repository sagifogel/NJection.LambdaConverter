using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static MemberReference MemberReference(NRefactory.MemberReferenceExpression memberReferenceExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new MemberReference(memberReferenceExpression, scope, visitor);
        }
    }
}