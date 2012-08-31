using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static New New(NRefactory.ObjectCreateExpression objectCreation, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new New(objectCreation, scope, visitor);
        }
    }
}