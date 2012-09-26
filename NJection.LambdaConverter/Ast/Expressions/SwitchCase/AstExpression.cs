using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Case SwitchCase(NRefactory.CaseLabel caseLabel, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Case(caseLabel, scope, visitor);
        }
    }
}