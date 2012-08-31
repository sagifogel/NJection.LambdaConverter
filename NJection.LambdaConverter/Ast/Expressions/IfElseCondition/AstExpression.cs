using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static IfElseCondition IfElseCondition(NRefactory.IfElseStatement IfElseStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new IfElseCondition(IfElseStatement, scope, visitor);
        }
    }
}