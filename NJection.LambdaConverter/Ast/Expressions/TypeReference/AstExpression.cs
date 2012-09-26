using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static TypeReference TypeReference(NRefactory.TypeReferenceExpression typeReferenceExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new TypeReference(typeReferenceExpression, scope, visitor);
        }
    }
}