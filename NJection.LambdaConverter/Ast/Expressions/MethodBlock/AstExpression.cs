using System.Collections.Generic;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static MethodBlock MethodBlock(NRefactory.BlockStatement blockStatement, IScope scope, INRefcatoryExpressionVisitor visitor, IEnumerable<ParameterExpression> parameters = null) {
            return new MethodBlock(blockStatement, parameters: parameters, scope: scope, visitor: visitor);
        }
    }
}