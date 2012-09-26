using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Default Default(NRefactory.DefaultValueExpression defaultValueExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Default(defaultValueExpression, scope, visitor);
        }
    }
}