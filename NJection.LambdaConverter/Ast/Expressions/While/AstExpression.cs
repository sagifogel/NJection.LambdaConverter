using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static While While(NRefactory.WhileStatement whileStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new While(whileStatement, scope, visitor);
        }
    }
}