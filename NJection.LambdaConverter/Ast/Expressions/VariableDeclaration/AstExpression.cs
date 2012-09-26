using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static VariableDeclaration Variable(NRefactory.VariableDeclarationStatement variableDeclaration, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new VariableDeclaration(variableDeclaration, scope, visitor);
        }
    }
}