using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static ParameterDecalration Parameter(NRefactory.ParameterDeclaration parameter, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new ParameterDecalration(parameter, scope, visitor);
        }
    }
}