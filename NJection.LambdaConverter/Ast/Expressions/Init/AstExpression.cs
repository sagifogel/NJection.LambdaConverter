using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Init Init(NRefactory.VariableInitializer variableInitializer, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Init(variableInitializer, scope, visitor);
        }
    }
}