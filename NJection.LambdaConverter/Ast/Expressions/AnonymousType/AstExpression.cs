using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static AnonymousType AnonymousType(NRefactory.AnonymousTypeCreateExpression anonymousTypeCreateExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new AnonymousType(anonymousTypeCreateExpression, scope, visitor);
        }
    }
}