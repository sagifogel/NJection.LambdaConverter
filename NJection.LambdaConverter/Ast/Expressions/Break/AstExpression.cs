using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Break Break(NRefactory.BreakStatement breakStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Break(breakStatement, scope, visitor);
        }
    }
}