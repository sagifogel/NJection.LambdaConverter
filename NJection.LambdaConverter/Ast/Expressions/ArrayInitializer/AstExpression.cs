using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static ArrayInitializer ArrayInitializer(NRefactory.ArrayInitializerExpression arrayInitializerExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new ArrayInitializer(arrayInitializerExpression, scope, visitor);
        }
    }
}