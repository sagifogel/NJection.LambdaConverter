using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Expressions;
using NJection.Scope;

namespace NJection.LambdaConverter.Visitors
{
    internal abstract class AbstractNRefcatoryAstVisitor : DepthFirstAstVisitor<IScope, AstExpression>, INRefcatoryExpressionVisitor
    {
    }
}