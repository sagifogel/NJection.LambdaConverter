using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Switch Switch(NRefactory.SwitchStatement switchStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Switch(switchStatement, scope, visitor);
        }
    }
}