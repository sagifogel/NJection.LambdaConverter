using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        protected static ForEach ForEach(NRefactory.ForeachStatement forEachStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new ForEach(forEachStatement, scope, visitor);
        }
    }
}