using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Expressions;
using NJection.Scope;

namespace NJection.LambdaConverter.Visitors
{
    public interface INRefcatoryExpressionVisitor : IAstVisitor<IScope, AstExpression>
    {
    }
}