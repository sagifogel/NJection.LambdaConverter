using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Expressions;

namespace NJection.LambdaConverter.Visitors
{
    internal abstract class AbstractNRefcatoryAstVisitor : DepthFirstAstVisitor<IScope, AstExpression>, INRefcatoryExpressionVisitor
    {
    }
}