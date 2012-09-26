using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Identifier Identifer(NRefactory.IdentifierExpression identifier, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Identifier(identifier, scope, visitor);
        }
    }
}