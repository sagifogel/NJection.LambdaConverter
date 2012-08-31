using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static TypeOf TypeOf(NRefactory.TypeOfExpression typeOfExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new TypeOf(typeOfExpression, scope, visitor);
        }
    }
}