using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static CaseLabel Label(NRefactory.CaseLabel caseLabel, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new CaseLabel(caseLabel, scope, visitor);
        }
    }
}