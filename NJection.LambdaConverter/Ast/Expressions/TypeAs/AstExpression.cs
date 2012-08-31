using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static TypeAs TypeAs(NRefactory.AsExpression asExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new TypeAs(asExpression, scope, visitor);
        }
    }
}