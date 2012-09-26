using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static ArrayCreation ArrayCreation(NRefactory.ArrayCreateExpression arrayCreateExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new ArrayCreation(arrayCreateExpression, scope, visitor);
        }
    }
}