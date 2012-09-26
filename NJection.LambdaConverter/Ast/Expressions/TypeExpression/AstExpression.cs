using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static TypeExpression TypeExpression(NRefactory.MemberType memberType, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new TypeExpression(memberType, scope, visitor);
        }

        public static TypeExpression TypeExpression(NRefactory.PrimitiveType primitiveType, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new TypeExpression(primitiveType, scope, visitor);
        }

        public static TypeExpression TypeExpression(NRefactory.SimpleType simpleType, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new TypeExpression(simpleType, scope, visitor);
        }

        public static TypeExpression TypeExpression(NRefactory.ComposedType composedType, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new TypeExpression(composedType, scope, visitor);
        }
    }
}