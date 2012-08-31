using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Primitive Primitive(NRefactory.PrimitiveExpression primitiveExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Primitive(primitiveExpression, scope, visitor);
        }
    }
}