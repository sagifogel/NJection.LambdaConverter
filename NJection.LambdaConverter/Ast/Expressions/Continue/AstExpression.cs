using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Continue Continue(NRefactory.ContinueStatement continueStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Continue(continueStatement, scope, visitor);
        }
    }
}