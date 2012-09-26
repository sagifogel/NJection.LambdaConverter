using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Binary Binary(NRefactory.BinaryOperatorExpression binaryOperatorExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Binary(binaryOperatorExpression, scope, visitor);
        }
    }
}