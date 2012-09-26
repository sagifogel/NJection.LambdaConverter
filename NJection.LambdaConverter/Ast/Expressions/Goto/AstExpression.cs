using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Goto Goto(NRefactory.GotoStatement gotoStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Goto(gotoStatement, scope, visitor);
        }
    }
}