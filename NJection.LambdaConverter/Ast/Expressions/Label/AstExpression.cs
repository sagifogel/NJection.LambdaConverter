using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Label Label(NRefactory.LabelStatement labelStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Label(labelStatement, scope, visitor);
        }
    }
}