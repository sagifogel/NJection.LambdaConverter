using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Expressions;

namespace NJection.LambdaConverter.Visitors
{
    public interface INRefcatoryExpressionVisitor : IAstVisitor<IScope, AstExpression>
    {
    }
}