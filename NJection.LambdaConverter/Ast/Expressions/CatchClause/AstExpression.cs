using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static CatchClause CatchClause(NRefactory.CatchClause CatchClause, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new CatchClause(CatchClause, scope, visitor);
        }
    }
}