using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Index Index(NRefactory.IndexerExpression indexerExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Index(indexerExpression, scope, visitor);
        }
    }
}